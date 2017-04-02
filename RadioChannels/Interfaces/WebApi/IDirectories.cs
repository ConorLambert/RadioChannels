using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioChannels.Interfaces.WebApi
{
    interface IDirectories
    {
        List<Channel> GetChannels(string genre, int index);
        List<Channel> GetChannels(string id);
    }
}
