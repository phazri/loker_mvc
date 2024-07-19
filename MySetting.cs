using AutoMapper;
using Microsoft.Extensions.Options;
using MyMiddleware;
using MyShared.MyException;
using MyShared.Service;
using SdmServiceApi.Loker.Interface;
using SdmServiceApi.Loker.Service;
using TempDtoShared.Jwt;
using TempDtoShared.Utility;

namespace LokerMVC;

internal static class MySetting
{
    public static IServiceCollection AddAppSettingService(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());//Setting Auto Mapper
        var config = new MapperConfiguration(x =>
        {
            x.AddMaps("SdmServiceApi");
            x.AddMaps("LokerMVC");
        });
        var mapper = config.CreateMapper();
        services.AddSingleton(mapper);

        var apiUrl = GetApiServiceUrl(services, configuration);
        SettingServiceApi(services, apiUrl);

        return services;
    }

    private static string GetApiServiceUrl(IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<ApiOptions>(configuration.GetSection("ApiUrlInternal"));
        services.Configure<JwtSetting>(configuration.GetSection("MyJwtSetting"));
        var urlApi = services.BuildServiceProvider().GetService<IOptions<ApiOptions>>()?.Value;

        if (urlApi?.ServiceApiLoker == null)
            throw new BadRequestException("Service Api Url Null");

        return urlApi.ServiceApiLoker;

    }

    private static void SettingServiceApi(IServiceCollection services, string apiEndPoint)
    {

        services.AddTransient<SetHeaderMiddleware>();
        services.AddHttpClient<IBiodata, JsonBiodataService>(x => { x.BaseAddress = new Uri(apiEndPoint); }).UseSetHeaderToken();
        services.AddHttpClient<ILogin, JsonLoginService>(x => { x.BaseAddress = new Uri(apiEndPoint); }).UseSetHeaderToken();
        services.AddHttpClient<ILoker, JsonLokerSevice>(x => { x.BaseAddress = new Uri(apiEndPoint); }).UseSetHeaderToken();
        services.AddHttpClient<IQuiz, JsonQuizService>(x => { x.BaseAddress = new Uri(apiEndPoint); }).UseSetHeaderToken();
        services.AddHttpClient<IUtility, UtilityService>();

    }
}

