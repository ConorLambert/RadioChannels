using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RadioChannels.Models
{
	public class Favourite
	{
        public int Id { get; set; }                
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public string ChannelName { get; set; }
    }
}