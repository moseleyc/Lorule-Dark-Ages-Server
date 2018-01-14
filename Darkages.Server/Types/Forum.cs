using Darkages.Network.ServerFormats;
using System;
using System.Collections.Generic;

namespace Darkages.Types
{
    public class Topic : Forum
    {
        public List<Post> Posts = new List<Post>();
    }

    public class Post
    {
        public ushort TopicNumber { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
        public DateTime DatePosted { get; set; }
        public bool Bold { get; set; }
    }

    public class Board : Forum
    {

    }

    public enum ForumConstraints
    {
        Default  = 0,
        MapClick = 1
    }

    public abstract class Forum : Template
    {
        public ForumConstraints Constraint { get; set; }

        public List<Topic> Topics = new List<Topic>();

        public int Number { get; set; }

        public string Title { get; set; }

        public void ListTopics(Aisling user)
        {
           user.Show(Scope.Self, new ServerFormat31(this));
        }
    }
}
