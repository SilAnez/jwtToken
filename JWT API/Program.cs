using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string key = "1uCpfKVEM7F7PnMJlZQ5SlduRbf8osyTNQxIktlT5KI=";

//AUTENTICADORES

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(opt =>
{
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);

    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,

        IssuerSigningKey = signingKey,
    };
}
);

var app = builder.Build();

//ENDPOINTS

app.MapGet("/protected", (ClaimsPrincipal user) => user.Identity?.Name)
    .RequireAuthorization();

app.MapGet("/auth/{user}/{pass}", (string user, string pass) => {

    if (user == "admin" && pass == "admin") {

        var tokenHandler = new JwtSecurityTokenHandler();
        var byteKey = Encoding.UTF8.GetBytes(key);
        var TokenDes = new SecurityTokenDescriptor
        {

            Subject = new ClaimsIdentity(new Claim[] {

                 new Claim(ClaimTypes.Name,user),


             }),

            Expires = DateTime.UtcNow.AddMonths(1), /*Token Expira en un mes*/
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey),
                                                            SecurityAlgorithms.HmacSha256Signature)
        };

        // Token para mandar al usuario
        var token = tokenHandler.CreateToken(TokenDes);
        return tokenHandler.WriteToken(token);
    }
    else
    {
        return "Usuario Inválido";
    }

});

app.Run();
