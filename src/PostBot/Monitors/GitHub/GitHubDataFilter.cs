using System;
using System.Collections.Generic;
using System.Linq;
using Octokit;

namespace PostBot.DataFilters
{
    public class GitHubDataFilter
    {
        private static readonly HashSet<string> SupportedActivityTypes = new HashSet<string>(StringComparer.Ordinal)
        {
            "IssuesEvent",
            "IssueCommentEvent",
            "PushEvent",
            "CreateEvent",
            "DeleteEvent",
            "PullRequestEvent",
            "PullRequestReviewCommentEvent",
            "CommitCommentEvent"
        };

        public IEnumerable<Activity> Filter(IEnumerable<Activity> data)
        {
            var filteredData = data.Where(activity => SupportedActivityTypes.Contains(activity.Type));

            return filteredData;
        }
    }
}
