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
            CreateMap<Email, EmailDto>();

            CreateMap<NewEmail, Email>();
        }
    }
}