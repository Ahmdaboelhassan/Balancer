using Domain.DTO.Both;
using Domain.Entities;

namespace Application.Mappers
{
    public static class BudgetAccountMapper
    {       /// <summary>
            /// Converts BudgetAccountSettings entity to BudgetAccountSettingsDTO
            /// </summary>
            /// <param name="entity">The BudgetAccountSettings entity to convert</param>
            /// <returns>BudgetAccountSettingsDTO object</returns>
        public static BudgetAccountSettingsDTO ToDTO(this BudgetAccount entity)
        {
            if (entity == null)
                return null;

            return new BudgetAccountSettingsDTO
            {
                Id = entity.Id,
                AccountId = entity.AccountId,
                Budget = entity.Budget,
                Color = entity.Color,
                DisplayName = entity.DisplayName
            };
        }

        /// <summary>
        /// Converts BudgetAccountSettingsDTO to BudgetAccountSettings entity
        /// </summary>
        /// <param name="dto">The BudgetAccountSettingsDTO to convert</param>
        /// <returns>BudgetAccountSettings entity object</returns>
        public static BudgetAccount ToEntity(this BudgetAccountSettingsDTO dto)
        {
            if (dto == null)
                return null;

            return new BudgetAccount
            {
                Id = dto.Id,
                AccountId = dto.AccountId,
                Budget = dto.Budget,
                Color = dto.Color,
                DisplayName = dto.DisplayName
            };
        }

        /// <summary>
        /// Converts a collection of BudgetAccountSettings entities to BudgetAccountSettingsDTO collection
        /// </summary>
        /// <param name="entities">Collection of BudgetAccountSettings entities</param>
        /// <returns>Collection of BudgetAccountSettingsDTO objects</returns>
        public static IEnumerable<BudgetAccountSettingsDTO> ToDTO(this IEnumerable<BudgetAccount> entities)
        {
            return entities?.Select(e => e.ToDTO()) ?? new List<BudgetAccountSettingsDTO>();
        }

        /// <summary>
        /// Converts a collection of BudgetAccountSettingsDTO to BudgetAccountSettings entity collection
        /// </summary>
        /// <param name="dtos">Collection of BudgetAccountSettingsDTO objects</param>
        /// <returns>Collection of BudgetAccountSettings entities</returns>
        public static IEnumerable<BudgetAccount> ToEntity(this IEnumerable<BudgetAccountSettingsDTO> dtos)
        {
            return dtos?.Select(dto => dto.ToEntity()) ?? new List<BudgetAccount>();
        }
    }
}
