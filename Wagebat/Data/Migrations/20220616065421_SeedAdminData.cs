using Microsoft.EntityFrameworkCore.Migrations;

namespace Wagebat.Data.Migrations
{
    public partial class SeedAdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            INSERT INTO[dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Discriminator]) VALUES(N'67456f6e-543f-490b-9c4b-38272c680ed6', N'admin', N'ADMIN', N'admin@wagebat.com', N'ADMIN@WAGEBAT.COM', 0, N'AQAAAAEAACcQAAAAEKM7+opimbSrIeQgu8vE0y4FiWkXsyimUMR5eLKi8jAUQmaC9Rs6j+eHeZVushjrSQ==', N'JOH4EXGICT5YZ44XO37VSNADXUGOOHQW', N'fc4102e2-8f93-412d-a392-fde50733d59a', N'01014581574', 0, 0, NULL, 1, 0, N'ApplicationUser')

            INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'c015a870-067f-4a32-9697-a3a6fb81f448', N'admin', N'ADMIN', N'61483ca5-ddeb-4e79-a1c3-6b91173bc231')

            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'67456f6e-543f-490b-9c4b-38272c680ed6', N'c015a870-067f-4a32-9697-a3a6fb81f448')

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
