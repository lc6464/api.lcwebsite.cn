var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddHttp304() // 添加 Http304 和 HttpConnectionInfo 服务
	.AddMemoryCache()
	.AddResponseCaching()
	.AddCors(options => {
		options.AddDefaultPolicy(policy => {
			policy.AllowAnyHeader();
			policy.AllowAnyMethod();
			policy.AllowCredentials();
			policy.SetPreflightMaxAge(TimeSpan.FromDays(1));
			policy.WithOrigins("https://lcwebsite.cn",
				"https://d.lcwebsite.cn",
				"https://test.lcwebsite.cn",
				"https://www.lcwebsite.cn"/*,
				"https://lc6464.cn",
				"https://d.lc6464.cn",
				"https://test.lc6464.cn",
				"https://www.lc6464.cn",
				"http://lc6464.cn",
				"http://d.lc6464.cn",
				"http://test.lc6464.cn",
				"http://www.lc6464.cn");*/
				);
		});
	}).AddResponseCompression(options => {
		options.EnableForHttps = true;
	}).AddControllers(options => {
		options.CacheProfiles.Add("Private30d", new() { Duration = 2592000, Location = ResponseCacheLocation.Client });
		options.CacheProfiles.Add("Public30d", new() { Duration = 2592000, Location = ResponseCacheLocation.Any });
		options.CacheProfiles.Add("Private1d", new() { Duration = 86400, Location = ResponseCacheLocation.Client });
		options.CacheProfiles.Add("Public1d", new() { Duration = 86400, Location = ResponseCacheLocation.Any });
		options.CacheProfiles.Add("Private1h", new() { Duration = 3600, Location = ResponseCacheLocation.Client });
		options.CacheProfiles.Add("Public1h", new() { Duration = 3600, Location = ResponseCacheLocation.Any });
		options.CacheProfiles.Add("Private10m", new() { Duration = 600, Location = ResponseCacheLocation.Client });
		options.CacheProfiles.Add("Private5m", new() { Duration = 300, Location = ResponseCacheLocation.Client });
		options.CacheProfiles.Add("Private1m", new() { Duration = 60, Location = ResponseCacheLocation.Client });
		options.CacheProfiles.Add("NoCache", new() { Duration = 0, Location = ResponseCacheLocation.None });
		options.CacheProfiles.Add("NoStore", new() { NoStore = true });
	});

builder.Logging.AddEventLog(eventLogSettings => {
	eventLogSettings.LogName = "api.lcwebsite.cn";
	eventLogSettings.SourceName = "Websites";
});

builder.Services.AddHttpClient("Timeout5s", client => {
	client.Timeout = new(0, 0, 5);
});

var app = builder.Build();

app.UseResponseCompression();

app.UseStaticFiles(new StaticFileOptions {
	OnPrepareResponse = context => context.Context.Response.Headers.CacheControl = "public,max-age=2592000" // 30天
});

app.MapControllers();

app.UseCors();

app.UseResponseCaching();

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

app.Run();