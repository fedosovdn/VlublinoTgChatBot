using VlublinoTgChatBot;
using VlublinoTgChatBot.WebApi.Options;
using VlublinoTgChatBot.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<TelegramReminderOptions>()
    .Bind(builder.Configuration.GetSection(TelegramReminderOptions.SectionName))
    .PostConfigure(options =>
    {
        var botToken = builder.Configuration["TG_BOT_TOKEN"];
        if (!string.IsNullOrWhiteSpace(botToken))
        {
            options.BotToken = botToken;
        }

        var chatIds = builder.Configuration["TG_CHAT_IDS"];
        if (!string.IsNullOrWhiteSpace(chatIds))
        {
            options.ChatIds = chatIds;
        }

        var scheduleCron = builder.Configuration["TG_SCHEDULE_CRON"];
        if (!string.IsNullOrWhiteSpace(scheduleCron))
        {
            options.ScheduleCron = scheduleCron;
        }

        var timeZoneId = builder.Configuration["TG_TIMEZONE"];
        if (!string.IsNullOrWhiteSpace(timeZoneId))
        {
            options.TimeZoneId = timeZoneId;
        }

        var message = builder.Configuration["TG_MESSAGE"];
        if (!string.IsNullOrWhiteSpace(message))
        {
            options.Message = message;
        }
    });
builder.Services.AddSingleton<ChatIdParser>();
builder.Services.AddSingleton<TimeZoneResolver>();
builder.Services.AddSingleton<TelegramReminderConfigBuilder>();
builder.Services.AddHostedService<TelegramReminderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
