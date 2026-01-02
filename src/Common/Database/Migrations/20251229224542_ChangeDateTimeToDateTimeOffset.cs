using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDateTimeToDateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailTexts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmailTexts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmailTexts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EmailTexts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EmailTexts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MessengerTexts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MessengerTexts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MessengerTexts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MessengerTexts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MessengerTexts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastLoginAt",
                table: "SystemUsers",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "SystemUsers",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "SupportTickets",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "SupportTickets",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ClosedAt",
                table: "SupportTickets",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "SupportTicketEntries",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "MessengerTexts",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "MessengerTexts",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "EmailTexts",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "EmailTexts",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SubscriptionValidUntil",
                table: "Customers",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastHeartbeat",
                table: "Customers",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Customers",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLoginAt",
                table: "SystemUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SystemUsers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "SupportTickets",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SupportTickets",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ClosedAt",
                table: "SupportTickets",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SupportTicketEntries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "MessengerTexts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MessengerTexts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "EmailTexts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EmailTexts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubscriptionValidUntil",
                table: "Customers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastHeartbeat",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

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
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", "admin@birthday-reminder.online", true, null, "$2a$11$KZOBamnRyzRqTG9RLOp9f.mOMetFx3G6Mw6.0ya5cP/6sh1VY06kC" });
        }
    }
}
