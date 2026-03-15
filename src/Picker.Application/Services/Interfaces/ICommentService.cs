using Picker.Application.DTOs.Comment;
using Picker.Domain.Enums;

namespace Picker.Application.Services.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetByItemAsync(Guid itemId, CategoryType categoryType);
    Task<CommentDto> GetByIdAsync(Guid id);
    Task<CommentDto> CreateAsync(CreateCommentDto dto);
    Task DeleteAsync(Guid id);
}
