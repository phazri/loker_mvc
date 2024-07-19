using AutoMapper;
using LokerMVC.Models;
using SdmServiceApi.Loker.Resources.Pelamar;
using TempDtoShared.Login;

namespace LokerMVC.Helper.Mapper;

public class PelamarMapper : Profile
{
    public PelamarMapper()
    {
        CreateMap<RegisterRequest, RegisterCalonPelamarDto>();
        CreateMap<RegisterCalonPelamarDto, RegisterRequest>();

        //Untuk Update Profile
        CreateMap<CalonPelamarRespond, ProfileRequest>();
        CreateMap<ProfileRequest, RegisterCalonPelamarDto>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => ""));


        CreateMap<CalonPelamarView, CalonPelamarRespond>();
        CreateMap<CalonPelamarRespond, CalonPelamarView>();

        CreateMap<PengalamanKerjaValidator, PengalamanKerjaDto>()
            .ForMember(dest => dest.LamaKerja, opt => opt.Ignore());

        CreateMap<PengalamanKerjaDto, PengalamanKerjaValidator>()
            .ForMember(dest => dest.LamaKerja, opt => opt.Ignore());


        CreateMap<ResponseQuiz, RespondQuizView>();
        CreateMap<RespondQuizView, ResponseQuiz>();

        CreateMap<FormIsianJawabanResponse, DataQuiz>()
            .ForMember(dest => dest.JawabanCollection, opt => opt.Ignore());

        CreateMap<DataQuiz, FormJawabanRequest>()
           .ForMember(dest => dest.Jawaban, opt => opt.MapFrom(src => src.Jawaban ?? string.Empty));
        
        CreateMap<FormIsianJawabanDetailResponses, PilihanJawaban>();
        CreateMap<PilihanJawaban, FormIsianJawabanDetailResponses>();


        CreateMap<GantiPassword, GantiPasswordRequest>()
            .ForMember(dest => dest.PasswordLama, opt => opt.MapFrom(src => src.PasswordLama ?? string.Empty))
            .ForMember(dest => dest.PasswordBaru, opt => opt.MapFrom(src => src.PasswordBaru));
    }
}