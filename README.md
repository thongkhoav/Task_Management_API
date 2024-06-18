- dotnet new webapi -o api
- dotnet watch run
- dotnet ef migrations add Init
- dotnet ef database update

INSERT INTO public."Tasks"(
"Id","Title", "Description", "DueDate", "IsComplete", "RoomId", "CreatedAt", "UpdatedAt")
VALUES ('b3783597-b4e2-4d8f-81e8-8d461fd60343','giat do', 'giat cho sach, cho ki', '2024-06-25 10:44:35.720362+07', false, '49531415-5331-414b-af8e-4f1f005e9172', current_timestamp, current_timestamp);
