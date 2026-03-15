using Picker.Application.DTOs.Comment;
using Picker.Domain.Enums;

namespace Picker.Application.Services.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetByItemAsync(Guid itemId, CategoryType categoryType);
    Task<CommentDto> GetByIdAsync(Guid id);
    Task<CommentDto> CreateAsync(CreateCommentDto dto, string userId);
    Task<CommentDto> UpdateAsync(Guid id, UpdateCommentDto dto, string userId, bool isAdmin);
    Task DeleteAsync(Guid id, string userId, bool isAdmin);
}
