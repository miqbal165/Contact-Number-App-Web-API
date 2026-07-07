namespace ContactNumberWebAPI.Common;

public enum ServiceResultStatus
{
    Success = 200,
    Created = 201,
    BadRequest = 400,
    Unauthorized = 401,
    NotFound = 404,
    Conflict = 409
}