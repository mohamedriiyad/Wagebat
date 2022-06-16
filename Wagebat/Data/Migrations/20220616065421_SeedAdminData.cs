using Microsoft.EntityFrameworkCore.Migrations;

namespace Wagebat.Data.Migrations
{
    public partial class SeedAdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Discriminator]) VALUES (N'121a58b9-5405-44a6-a69a-4bf75bd5b9fe', N'admin', N'ADMIN', N'admin@wagebat.com', N'ADMIN@WAGEBAT.COM', 0, N'AQAAAAEAACcQAAAAEJWevxtd1JyK6jLS4qtgXwjr1RcCzFqe8qGT/5GBLEJGXITyh3w+z5prXKCvCtaIIQ==', N'LRS7IW2OXAO6LHYID6SBEQJGAR56WAZN', N'6f658413-a81d-4d52-83fc-49f0bd5d0481', N'0120000000', 0, 0, NULL, 1, 0, N'ApplicationUser')
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'8051df92-b636-4af1-91ac-72dd326bde92', N'admin', N'ADMIN', N'9aa1ec96-bcdc-4b2a-b41e-815eb5e96548')
INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'121a58b9-5405-44a6-a69a-4bf75bd5b9fe', N'8051df92-b636-4af1-91ac-72dd326bde92')
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
