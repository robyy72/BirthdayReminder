using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportTicketSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PurchaseToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Store = table.Column<int>(type: "int", nullable: false),
                    PreferredChannel = table.Column<int>(type: "int", nullable: false),
                    Subscription = table.Column<int>(type: "int", nullable: false),
                    SubscriptionValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastHeartbeat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsHtml = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessengerTexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    PreviewText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessengerTexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SystemUserId = table.Column<int>(type: "int", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTickets_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportTickets_SystemUsers_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupportTicketId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFromCustomer = table.Column<bool>(type: "bit", nullable: false),
                    SystemUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTicketEntries_SupportTickets_SupportTicketId",
                        column: x => x.SupportTicketId,
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportTicketEntries_SystemUsers_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "EmailTexts",
                columns: new[] { "Id", "Content", "CreatedAt", "IsActive", "IsHtml", "Name", "Subject", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "<h1>Welcome to Birthday Reminder!</h1>\r\n<p>Thank you for joining Birthday Reminder. We're excited to help you never forget an important birthday again.</p>\r\n<p>Get started by adding your first birthday reminder in the app.</p>\r\n<p>Best regards,<br>The Birthday Reminder Team</p>", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Welcome", "Welcome to Birthday Reminder!", null },
                    { 2, "<h1>Birthday Reminder</h1>\r\n<p>Don't forget! <strong>{Name}</strong>'s birthday is on <strong>{Date}</strong>.</p>\r\n<p>You have {DaysLeft} days to prepare a gift or send your wishes.</p>\r\n<p>Best regards,<br>The Birthday Reminder Team</p>", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Birthday Reminder", "Birthday Reminder: {Name}'s birthday is coming up!", null },
                    { 3, "<h1>Support Request Received</h1>\r\n<p>Thank you for contacting us. We have received your support request.</p>\r\n<p><strong>Ticket ID:</strong> #{TicketId}<br>\r\n<strong>Subject:</strong> {Subject}</p>\r\n<p>Our team will review your request and get back to you as soon as possible.</p>\r\n<p>Best regards,<br>The Birthday Reminder Support Team</p>", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Support Ticket Created", "Support Ticket #{TicketId} - We received your request", null },
                    { 4, "<h1>New Response to Your Support Ticket</h1>\r\n<p>There is a new response to your support ticket #{TicketId}.</p>\r\n<p><strong>Response:</strong></p>\r\n<blockquote>{Message}</blockquote>\r\n<p>If you have any further questions, please reply to this email.</p>\r\n<p>Best regards,<br>The Birthday Reminder Support Team</p>", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Support Ticket Response", "Support Ticket #{TicketId} - New response", null },
                    { 5, "<h1>Support Ticket Closed</h1>\r\n<p>Your support ticket #{TicketId} has been closed.</p>\r\n<p>If you need further assistance, feel free to open a new support request in the app.</p>\r\n<p>Thank you for using Birthday Reminder!</p>\r\n<p>Best regards,<br>The Birthday Reminder Support Team</p>", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Support Ticket Closed", "Support Ticket #{TicketId} - Closed", null }
                });

            migrationBuilder.InsertData(
                table: "MessengerTexts",
                columns: new[] { "Id", "Channel", "Content", "CreatedAt", "IsActive", "Name", "PreviewText", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 3, "Hey! Just a friendly reminder: {Name}'s birthday is on {Date}. You have {DaysLeft} days left to prepare!", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Birthday Reminder WhatsApp", "Birthday reminder for {Name}", null },
                    { 2, 2, "Birthday Reminder: {Name}'s birthday is on {Date}. {DaysLeft} days left!", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Birthday Reminder SMS", "Birthday: {Name}", null },
                    { 3, 4, "Hey! Just a friendly reminder: {Name}'s birthday is on {Date}. You have {DaysLeft} days left to prepare!", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Birthday Reminder Signal", "Birthday reminder for {Name}", null },
                    { 4, 3, "New response to your support ticket #{TicketId}:\n\n{Message}\n\nReply in the app for further assistance.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Support Response WhatsApp", "Support Ticket #{TicketId}", null },
                    { 5, 2, "Ticket #{TicketId} update: {Message}", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Support Response SMS", "Ticket #{TicketId}", null }
                });

            migrationBuilder.InsertData(
                table: "SystemUsers",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Email", "IsActive", "LastLoginAt", "PasswordHash" },
                values: new object[] { 1, new DateTime(2025, 12, 29, 22, 32, 4, 984, DateTimeKind.Utc).AddTicks(7501), "Admin", "admin@birthday-reminder.online", true, null, "$2a$11$KZOBamnRyzRqTG9RLOp9f.mOMetFx3G6Mw6.0ya5cP/6sh1VY06kC" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketEntries_SupportTicketId",
                table: "SupportTicketEntries",
                column: "SupportTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketEntries_SystemUserId",
                table: "SupportTicketEntries",
                column: "SystemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_CreatedAt",
                table: "SupportTickets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_CustomerId",
                table: "SupportTickets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_Status",
                table: "SupportTickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_SystemUserId",
                table: "SupportTickets",
                column: "SystemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsers_Email",
                table: "SystemUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTexts");

            migrationBuilder.DropTable(
                name: "MessengerTexts");

            migrationBuilder.DropTable(
                name: "SupportTicketEntries");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "SystemUsers");
        }
    }
}
