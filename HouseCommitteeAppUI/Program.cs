using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using HouseCommitteeAppUI;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);



builder.ConfigureServices();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

/*when signing out, we want to go back to the home page instead of being left in a boring page that only signs you out*/
//TODO : check why the redirection is not working
app.UseRewriter(
    new RewriteOptions().Add(
        context =>
        {
            if (context.HttpContext.Request.Path == "/MicrosoftIdentity/Account/SignedOut")
            {
                context.HttpContext.Response.Redirect("/");
            }
        })
    );

app.MapControllers();   
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
