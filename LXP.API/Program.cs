using System.Reflection;
using FluentValidation.AspNetCore;
using LXP.API.Interceptors;
using LXP.Common.Entities;
using LXP.Common.Validators;
using LXP.Core.IServices;
using LXP.Core.Repositories;
using LXP.Core.Services;
using LXP.Data.IRepository;
using LXP.Data.Repository;
using LXP.Services;
using LXP.Services.IServices;
using OfficeOpenXml;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region CORS setting for API
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "_myAllowSpecificOrigins",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowAnyMethod();
        }
    );
});

#endregion


builder.Services.AddScoped<LXPDbContext>();

builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuizQuestionService, QuizQuestionService>();
builder.Services.AddScoped<IQuizQuestionRepository, QuizQuestionRepository>();
builder.Services.AddScoped<IBulkQuestionRepository, BulkQuestionRepository>();
builder.Services.AddScoped<IBulkQuestionService, BulkQuestionService>();
builder.Services.AddScoped<IQuizFeedbackService, QuizFeedbackService>();
builder.Services.AddScoped<IQuizFeedbackRepository, QuizFeedbackRepository>();
builder.Services.AddScoped<ITopicFeedbackRepository, TopicFeedbackRepository>();
builder.Services.AddScoped<IQuizEngineRepository, QuizEngineRepository>();
builder.Services.AddScoped<IQuizEngineService, QuizEngineService>();
builder.Services.AddScoped<IQuizFeedbackService, QuizFeedbackService>();
builder.Services.AddScoped<ITopicFeedbackService, TopicFeedbackService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IFeedbackResponseRepository, FeedbackResponseRepository>();
builder.Services.AddScoped<IFeedbackResponseService, FeedbackResponseService>();

//new
builder.Services.AddScoped<IExcelToJsonService, ExcelToJsonService>();
builder.Services.AddScoped<IQuizQuestionJsonRepository, QuizQuestionJsonRepository>();

builder.Services.AddSingleton<LXPDbContext>();

builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(
        Path.Combine(
            AppContext.BaseDirectory,
            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
        )
    );
});

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//builder.Services.AddMvc(options =>
//{
//    options.Filters.Add<ApiExceptionInterceptor>();
//});

builder
    .Services.AddControllers()
    .AddFluentValidation(v =>
    {
        v.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });

builder.Services.AddTransient<BulkQuizQuestionViewModelValidator>();
builder.Services.AddTransient<TopicFeedbackResponseViewModelValidator>();
builder.Services.AddTransient<QuizFeedbackResponseViewModelValidator>();
builder.Services.AddMemoryCache();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseCors("_myAllowSpecificOrigins");
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
