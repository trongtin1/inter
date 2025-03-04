using AutoMapper;
using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;
using test.Project.Domain.Entity;
using test.Project.Application.DTOs.Response.Notification;

using test.Project.Infrastructure.Extensions;


namespace test.Project.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IContextService _userContextService;

        public NotificationService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        //public async Task<ApiResponse<PagedResult<Notification>>> GetNotificationsAsync(
        //    int pageIndex,
        //    int pageSize,
        //    string? id,
        //    string? type,
        //    bool? isRead,
        //    string? email,
        //    string? from,
        //    bool? isSeen,
        //    string? timeType,
        //    DateTime? fromDate,
        //    DateTime? toDate)
        //{
        //    try
        //    {
        //        var userEmail = _userContextService.GetUserEmail();
        //        var rolesList = _userContextService.GetUserRoles();

        //        if (!rolesList.Contains("admin"))
        //        {
        //            email = userEmail;
        //        }

        //        var (items, totalItems) = await _unitOfWork.Notifications.GetNotificationsAsync(
        //            pageIndex, pageSize, id, type, isRead, email, from, isSeen, 
        //            timeType, fromDate, toDate);

        //        var pagedResult = new PagedResult<Notification>
        //        {
        //            Items = items,
        //            TotalItems = totalItems,
        //            PageIndex = pageIndex,
        //            PageSize = pageSize
        //        };

        //        return new ApiResponse<PagedResult<Notification>>
        //        {
        //            Success = true,
        //            Data = pagedResult
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<PagedResult<Notification>>
        //        {
        //            Success = false,
        //            Message = ex.Message
        //        };
        //    }
        //}
        public async Task<ApiResponse<PagedResult<Notification>>> GetPageNotification( 
            string? id,
            string? type,
            bool? isRead,
            string? email,
            string? from,
            bool? isSeen,
            string? timeType,
            DateTime? fromDate,
            DateTime? toDate,
            int? pageIndex,
            int? pageSize)
        {
            try
            {
                int finalPageIndex = pageIndex ?? 1;
                int finalPageSize = pageSize ?? 10;
                var userEmail = _userContextService.GetUserEmail();
                var rolesList = _userContextService.GetUserRoles();
                if (!rolesList.Contains("admin"))
                {
                    email = userEmail;
                }

                var (items, totalCount) = await _unitOfWork.Notifications.GetPageNotification(
                    id, type, isRead, email, from, isSeen, timeType, fromDate, toDate, finalPageIndex, finalPageSize
                    );

                var notiRes = _mapper.Map<IEnumerable<Notification>>(items);

                var pagedResult = new PagedResult<Notification>
                {
                    Items = notiRes,
                    TotalItems = totalCount,
                    PageIndex = finalPageIndex,
                    PageSize = finalPageSize
                };

                return new ApiResponse<PagedResult<Notification>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = pagedResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<Notification>>
                {
                    Success = false,
                    Message = "Error retrieving users: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<Notification>> GetNotificationByIdAsync(
            long id)
        {
            try
            {
                var userEmail = _userContextService.GetUserEmail();
                var rolesList = _userContextService.GetUserRoles();
                var notification = await _unitOfWork.Notifications.GetByIdAsync(id);

                if (notification == null || !rolesList.Contains("admin"))
                {
                    return new ApiResponse<Notification>
                    {
                        Success = false,
                        Message = "Access denied",
                        Data = null
                    };
                }

                return new ApiResponse<Notification>
                {
                    Success = true,
                    Message = "Notification fetched successfully",
                    Data = notification
                };
            }
            catch (Exception)
            {
                return new ApiResponse<Notification>
                {
                    Success = false,
                    Message = "Error fetching notification"
                };
            }
        }

        public async Task<ApiResponse<Notification>> CreateNotificationAsync(Notification notification)
        {
            try
            {
                var result = await _unitOfWork.Notifications.CreateAsync(notification);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<Notification>
                {
                    Success = true,
                    Message = "Notification created successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Notification>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<Notification>> UpdateNotificationAsync(
            long id,
            Notification notification)
        {
            try
            {
                var userEmail = _userContextService.GetUserEmail();
                var rolesList = _userContextService.GetUserRoles();
                var existingNotification = await _unitOfWork.Notifications.GetByIdAsync(id);

                if (existingNotification == null)
                {
                    return new ApiResponse<Notification>
                    {
                        Success = false,
                        Message = "Notification not found",
                        Data = null
                    };
                }

                notification.CreatedTime = existingNotification.CreatedTime;
                notification.LastModified = DateTime.Now;
                _mapper.Map(notification, existingNotification);
                
                var result = await _unitOfWork.Notifications.UpdateAsync(existingNotification);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<Notification>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Notification>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> DeleteNotificationAsync(long id)
        {
            try
            {
                var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
                if (notification == null)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Notification not found"
                    };
                }

                await _unitOfWork.Notifications.DeleteAsync(notification);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Notification deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> DeleteMultipleNotificationsAsync(List<long> ids)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.GetByIdsAsync(ids);
                await _unitOfWork.Notifications.DeleteRangeAsync(notifications);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<object>
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> GetFilterOptionsAsync()
        {
            try
            {
                var userEmail = _userContextService.GetUserEmail();
                var rolesList = _userContextService.GetUserRoles();
                var data = await _unitOfWork.Notifications.GetFilterDataAsync();

                var filterOptions = new NotificationFiltersRes
                {
                    Ids = data.Select(m => m.Id.ToString()).Distinct().ToList(),
                    Emails = data.Select(m => m.Email).ToList(),
                    Froms = data.Select(m => m.From).ToList(),
                    Types = data.Select(m => m.Type).Distinct().ToList()
                };

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Notification filter options fetched successfully",
                    Data = filterOptions
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<Notification>> GetOneNotification(string? id, string? type, bool? isRead, string? email, string? from, bool? isSeen, string? timeType, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var userEmail = _userContextService.GetUserEmail();
                var rolesList = _userContextService.GetUserRoles();
                if (!rolesList.Contains("admin"))
                {
                    email = userEmail;
                }

                var noti = await _unitOfWork.Notifications.GetOneNotification(id, type, isRead, email, from, isSeen, timeType, fromDate, toDate);
                if (noti == null)
                {
                    return new ApiResponse<Notification>
                    {
                        Success = false,
                        Message = "Noti not found"
                    };
                }

                var notiRes = _mapper.Map<Notification>(noti);
                return new ApiResponse<Notification>
                {
                    Success = true,
                    Message = "User retrieved successfully",
                    Data = notiRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Notification>
                {
                    Success = false,
                    Message = "Error retrieving user: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<Notification>>> GetListNotification(
            string? id, 
            string? type, 
            bool? isRead, 
            string? email, 
            string? from, 
            bool? isSeen, 
            string? timeType, 
            DateTime? fromDate, 
            DateTime? toDate)
        {
            try
            {
                var userEmail = _userContextService.GetUserEmail();
                var rolesList = _userContextService.GetUserRoles();
                if (!rolesList.Contains("admin"))
                {
                    email = userEmail;
                }
                var noti = await _unitOfWork.Notifications.GetListNotification(id, type, isRead, email, from, isSeen, timeType, fromDate, toDate);
                var notiRes = _mapper.Map<List<Notification>>(noti);

                return new ApiResponse<List<Notification>>
                {
                    Success = true,
                    Message = "Notifications retrieved successfully",
                    Data = notiRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Notification>>
                {
                    Success = false,
                    Message = "Error retrieving notifications: " + ex.Message
                };
            }
        }
    }
} 