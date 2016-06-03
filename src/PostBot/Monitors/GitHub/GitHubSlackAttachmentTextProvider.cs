using System;
using Microsoft.Extensions.Logging;
using Octokit;
using PostBot.Configuration;

namespace PostBot.Monitors.GitHub
{
    public class GitHubSlackAttachmentTextProvider
    {
        private readonly GithubConfiguration _configuration;
        private readonly ILogger _logger;

        public GitHubSlackAttachmentTextProvider(ILogger logger, GithubConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public bool TryGetAttachmentText(Activity activity, out string attachmentText)
        {
            attachmentText = null;

            var repoName = activity.Repo.Name;
            if (repoName.StartsWith(_configuration.Organization + "/", StringComparison.OrdinalIgnoreCase))
            {
                repoName = repoName.Substring(_configuration.Organization.Length + 1);
            }
            var textStart = $"[<https://github.com/{activity.Repo.Name}|{repoName}>] *{SE(activity.Actor.Login)}* has";

            switch (activity.Type)
            {
                case "IssuesEvent":
                    var issueEventPayload = activity.Payload as IssueEventPayload;

                    if (issueEventPayload == null)
                    {
                        _logger.LogError("Payload was unrecognizable for IssuesEvent.");
                        return false;
                    }

                    attachmentText = $"{textStart} *{SE(issueEventPayload.Action)}* <{issueEventPayload.Issue.HtmlUrl}|issue> `" +
                        $"{SE(issueEventPayload.Issue.Title)}`";

                    return true;
                case "IssueCommentEvent":
                    var issueCommentEventPayload = activity.Payload as IssueCommentPayload;

                    if (issueCommentEventPayload == null)
                    {
                        _logger.LogError("Payload was unrecognizable for IssueCommentEvent.");
                        return false;
                    }

                    if (issueCommentEventPayload.Issue.PullRequest != null)
                    {
                        attachmentText = $"{textStart} <{issueCommentEventPayload.Comment.HtmlUrl}|commented> on " +
                            $"*{SE(issueCommentEventPayload.Issue.User.Login)}*'s <{issueCommentEventPayload.Issue.HtmlUrl}|pull request> " +
                            $"`{SE(issueCommentEventPayload.Issue.Title)}`";
                    }
                    else
                    {
                        attachmentText = $"{textStart} <{issueCommentEventPayload.Comment.HtmlUrl}|commented> on <{issueCommentEventPayload.Issue.HtmlUrl}|issue> " +
                            $"`{SE(issueCommentEventPayload.Issue.Title)}`";
                    }
                    return true;
                case "PushEvent":
                    var pushPayload = activity.Payload as PushEventPayload;

                    if (pushPayload == null)
                    {
                        _logger.LogError("Payload was unrecognizable for PushEvent.");
                        return false;
                    }

                    var branch = pushPayload.Ref.Replace("refs/heads/", string.Empty);
                    var branchCommitsUrl = "https://github.com/" + activity.Repo.Name + "/commits/" + branch;
                    var branchLink = $"<{branchCommitsUrl}|{branch}>";

                    if (pushPayload.Commits.Count > 1)
                    {
                        attachmentText = $"{textStart} pushed {pushPayload.Commits.Count} commits to {branchLink}";
                    }
                    else
                    {
                        var commit = pushPayload.Commits[0];
                        var commitMessage = commit.Message.Split('\r', '\n')[0];
                        var branchCommitUrl = "https://github.com/" + activity.Repo.Name + "/commit/" + commit.Sha;
                        attachmentText = $"{textStart} pushed <{branchCommitUrl}|commit> `{SE(commitMessage)}` to {branchLink}";
                    }

                    return true;
                case "PullRequestEvent":
                    var pullRequestPayload = activity.Payload as PullRequestEventPayload;
                    if (pullRequestPayload == null)
                    {
                        _logger.LogError("Payload was unrecognizable for PullRequestEvent.");
                        return false;
                    }

                    var branchHead = pullRequestPayload.PullRequest.Head;
                    var branchBase = pullRequestPayload.PullRequest.Base;
                    var branchUrlStart = "https://github.com/" + activity.Repo.Name + "/tree/";

                    attachmentText = $"{textStart} *{SE(pullRequestPayload.Action)}* <{pullRequestPayload.PullRequest.HtmlUrl}|pull request> " +
                        $"`{SE(pullRequestPayload.PullRequest.Title)}` from <{branchUrlStart}{branchHead.Ref}|{branchHead.Ref}> =&gt; " +
                        $"<{branchUrlStart}{branchBase.Url}|{branchBase.Ref}>";
                    return true;
                case "PullRequestReviewCommentEvent":
                    var pullRequestCommentPayload = activity.Payload as PullRequestCommentPayload;
                    if (pullRequestCommentPayload == null)
                    {
                        _logger.LogError("Payload was unrecognizable for PullRequestReviewCommentEvent.");
                        return false;
                    }

                    attachmentText = $"{textStart} <{pullRequestCommentPayload.Comment.HtmlUrl}|commented> on " +
                            $"*{SE(pullRequestCommentPayload.PullRequest.User.Login)}*'s " +
                            $"<{pullRequestCommentPayload.PullRequest.HtmlUrl}|pull request> " +
                            $"`{SE(pullRequestCommentPayload.PullRequest.Title)}`";
                    return true;
                case "CommitCommentEvent":
                    var commitCommentPayload = activity.Payload as CommitCommentPayload;

                    attachmentText = $"{textStart} commented on a <{commitCommentPayload.Comment.HtmlUrl}|commit>";
                    return true;
            }

            return false;
        }

        private static string SE(string stringToSlackEncode)
        {
            stringToSlackEncode = stringToSlackEncode
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");

            return stringToSlackEncode;
        }
    }
}
