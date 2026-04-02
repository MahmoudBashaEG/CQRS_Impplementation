using CQRS.API.Middlewares;
using CQRS.Application;
using CQRS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// -------------------------------------------------------------------------------------------------------------------
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // Serves the Swagger UI (HTML, JS, CSS)
    app.UseSwaggerUI(options =>
    {
        // This is the default path to the generated JSON
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();















/*We start with small code, cause the business is simple and easy. Here, we don't need CQRS because it doesn't give use benefits.

But, when the business requires complex features etc. The code starts get bigger and complex. Also, when the business goes viral and many users come then we need scalable system. Here, we need to implement CQRS that helps use building a good system architecture.



So, i think the benefits of CQRS:



1.Maintain small portion of codes (use case) not one huge, big service. That helps code maintainability.



2. Test a specific use case easily.



3. Dependencies, when all the code inside one service, you need many dependencies which many not useful for each action at the service which makes big object size in Memory, when mocking dependencies in testing, you mocking dozens of services and the test code's going to be not manageable.



4. Make the performance for reading is fast cause we DTO, CQRS by nature forcing u to use DTO, but without CQRS you may or may not use DTOs.



5. Make the domain entity small, maintainable, and not polluted with UI methods like format Customer name etc. cause the domain entity by nature is responsible for the state, business behaviors, and business rules.



6. After doing this separation, we did theoretical separation at the code, but when we need high scalability, if we implemented CQRS, we could easily do physical separation by separate the Command & Query into 2 diff projects and deploy them into different servers. But if the code was in one big service we can't do this.



7. When we need high scalability and the database traffic most of it is reading, we need many indexes, but the indexes slow down the write, also when we write we need exclusive locks and those locks slow down the read. Now, we need database for writing and sync it with fast database for reads by fire events or something and do indexes at those read database. and those read database can be many of them "replicas" to handle high traffic.

So, if we implemented CQRS, this helps us do this easily by using the read database at the Query part and use the write database and the command part.





So, implementing CQRS helping us at complex code and scalability by providing this separation make us able to build around it, a specific architecture that can make such system works very well.

*/