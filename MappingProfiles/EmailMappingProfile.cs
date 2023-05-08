using AutoMapper;
using EmailWeb.Data;
using EmailWeb.Models;

namespace EmailWeb.MappingProfiles
{
    public class EmailMappingProfile : Profile
    {
        public EmailMappingProfile()
        {
            CreateMap<EmailDto, Email>();
            CreateMap<Email, EmailDto>()
                .ForMember(e => e.CreatedAt, o => o.MapFrom(e => e.CreatedAt))
                .ForMember(e => e.Id, o => o.MapFrom(e => e.Id));

            CreateMap<NewEmail, Email>();
        }
    }
}