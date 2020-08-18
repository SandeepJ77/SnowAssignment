using AutoMapper;
using NATS.Client;
using Newtonsoft.Json;
using System.Text;

namespace Snow.Utility.Models
{
    public class Message
    {
        public string Subject { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
    }

    public class MessageMapperProfile : Profile
    {
        public MessageMapperProfile()
        {
            CreateMap<Msg, Message>()
               .ForPath(dest => dest.Subject, opt => opt.MapFrom(x => x.Subject))
               .ForPath(dest => dest.Sender, opt => opt.MapFrom(x => x.Reply))
               .ForPath(dest => dest.Content, opt => opt.MapFrom(x => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(x.Data, 0, x.Data.Length))));
        }
    }
}
