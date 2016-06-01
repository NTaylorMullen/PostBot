﻿using System;

namespace PostBot.Configuration
{
    public class SlackConfiguration
    {
        private string _postChannel;

        public string PostChannel
        {
            get
            {
                return _postChannel;
            }
            set
            {
                if (value[0] != '#')
                {
                    _postChannel = $"#{value}";
                }
                else
                {
                    _postChannel = value;
                }
            }
        }

        public Uri WebHookUrl { get; set; }

        public int MessageBufferSize { get; set; }

        public string MessageDeletionToken { get; set; }

        public string SlackApiUrl { get; set; }
    }
}