var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddEventLog(eventLogSettings => {
	eventLogSettings.LogName = "api.lcwebsite.cn";
	eventLogSettings.SourceName = "Websites";
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddCors(options => {
	options.AddDefaultPolicy(policy => {
		policy.AllowAnyHeader();
		policy.AllowAnyMethod();
		policy.AllowCredentials();
		policy.SetPreflightMaxAge(new(24, 0, 0));
		policy.WithOrigins("https://lcwebsite.cn",
			"https://d.lcwebsite.cn",
			"https://test.lcwebsite.cn",
			"https://www.lcwebsite.cn",
			"https://lc6464.cn",
			"https://d.lc6464.cn",
			"https://test.lc6464.cn",
			"https://www.lc6464.cn",
			"http://lc6464.cn",
			"http://d.lc6464.cn",
			"http://test.lc6464.cn",
			"http://www.lc6464.cn");
	});
});

builder.Services.AddHttpClient("Timeout5s", httpClient => {
	httpClient.Timeout = new(0, 0, 5);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();

app.MapControllers();

app.UseCors();

app.UseAuthorization();

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

app.Run();