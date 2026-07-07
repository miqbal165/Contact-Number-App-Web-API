using AutoMapper;
using ContactNumberWebAPI.DTOs.ContactCategories;
using ContactNumberWebAPI.DTOs.Contacts;
using ContactNumberWebAPI.Models;

namespace ContactNumberWebAPI.Settings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ContactCategory, ContactCategoryResponse>();
        CreateMap<ContactCategoryCreateRequest, ContactCategory>();
        CreateMap<ContactCategoryUpdateRequest, ContactCategory>();

        CreateMap<Contact, ContactResponse>()
            .ForMember(
                destination => destination.ContactCategoryName,
                option => option.MapFrom(source => source.ContactCategory 
                                                   == null ? string.Empty : source.ContactCategory.Name));

        CreateMap<ContactCreateRequest, Contact>();
        CreateMap<ContactUpdateRequest, Contact>();
    }
}