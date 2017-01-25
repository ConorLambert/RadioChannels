using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadioChannels.Models
{
	public class Favourite
	{
        public int ID { get; set; }
        public int UserId { get; set; }
        public string ChannelName { get; set; }
    }
}