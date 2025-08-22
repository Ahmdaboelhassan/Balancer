using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;

namespace Application.Services
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IUnitOfWork _uow;
        public EvaluationService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<EvaluationListItemDTO>> GetAll(DateTime From, DateTime To)
        {
            var evaluations = await _uow.Evaluations.GetAll(p => p.From >= From.Date && p.From <= To.Date);
            return evaluations.OrderByDescending(p => p.To).Select(sp => new EvaluationListItemDTO
            {
                Id = sp.Id,
                Name = sp.Name,
                From = sp.From.ToShortDateString(),
                To = sp.To.ToShortDateString(),
                Profit = sp.Profit,
                IncreasedProfit = sp.Profit >= 0,
            });
        }

        public async Task<EvaluationDTO?> New()
        {
            var lastEvaluation = await _uow.Evaluations.GetLastOrderBy(p => p.Id, "EvaluationDetails");
            var fromDate = new DateTime(DateTime.Now.Year, 1, 1);

            var details = new List<EvaluationDetailDTO>();
            if (lastEvaluation != null && lastEvaluation.EvaluationDetails != null)
            {
                details = lastEvaluation.EvaluationDetails.Select(d => new EvaluationDetailDTO
                {
                    AccountId = d.AccountId,
                    Amount = 0,
                    Percentage = 0,
                }).ToList();

                fromDate = lastEvaluation.To;
            }

            var toDate = fromDate.AddMonths(3);

            var count = await _uow.Evaluations.Count(p => p.From.Year == DateTime.Now.Year);

            return new EvaluationDTO()
            {
                From = fromDate.ToShortDateString(),
                To = toDate.ToShortDateString(),
                Name = $"Evaluation Number {count + 1}# In {fromDate:yyy}",
                EvaluationDetails = details,
            };
        }

        public async Task<EvaluationDTO> GetById(int id)
        {
            var eva = await _uow.Evaluations.Get(p => p.Id == id, "EvaluationDetails");

            if (eva == null)
                return new EvaluationDTO();

            var evaluationDTO = new EvaluationDTO
            {
                Id = eva.Id,
                From = eva.From.ToShortDateString(),
                To = eva.To.ToShortDateString(),
                Income = eva.Income,
                Name = eva.Name,
                LastUpdatedAt = eva.LastUpdatedAt.ToString("g"),
                CreatedAt = eva.LastUpdatedAt.ToString("g"),
                Note = eva.Note,
                Profit = eva.Profit,
                ProfitPercentage = Math.Round((eva.Profit / eva.Income) * 100, 2),
                EvaluationDetails = eva.EvaluationDetails.Select(d => new EvaluationDetailDTO
                {
                    AccountId = d.AccountId,
                    Amount = d.Amount,
                    Percentage = Math.Round((d.Amount / eva.Income) * 100, 2),
                }).OrderByDescending(d => d.Amount)
            };

            return evaluationDTO;
        }

        public async Task<Result<EvaluationDTO>> CalculateDetailsBalances(EvaluationDTO DTO)
        {
            DateTime from, to;

            if (DateTime.TryParse(DTO.From, out DateTime parsedFrom) && DateTime.TryParse(DTO.To, out DateTime parsedTo))
            {
                from = parsedFrom;
                to = parsedTo;
            }
            else
            {
                return new Result<EvaluationDTO> { IsSucceed = false, Message = "Invalid Date Format" };
            }

            if (DTO.EvaluationDetails == null || !DTO.EvaluationDetails.Any())
            {
                return new Result<EvaluationDTO> { IsSucceed = false, Message = "No Details Provided" };
            }

            if (DTO.Income <= 0)
                return new Result<EvaluationDTO> { IsSucceed = false, Message = "Income Should Be Grater Than Zero" };


            var accountsId = DTO.EvaluationDetails.Select(d => d.AccountId).Distinct().ToList();

            var accounts = await _uow.Accounts.GetAll(a => accountsId.Contains(a.Id));

            var journalsDetails = await _uow.JournalDetail.GetAll(j => j.Journal.CreatedAt.Date >= from.Date && j.Journal.CreatedAt.Date <= to.Date, "Journal", "Account");


            var evaluationDetailDTOs = new List<EvaluationDetailDTO>(DTO.EvaluationDetails.Count());

            foreach (var account in accounts)
            {
                var amount = journalsDetails.Where(d => d.Account.Number.StartsWith(account.Number)).Sum(d => (d.Debit - d.Credit));

                evaluationDetailDTOs.Add(new EvaluationDetailDTO
                {
                    AccountId = account.Id,
                    Amount = amount,
                    Percentage = Math.Round((amount / DTO.Income) * 100, 2)
                });

            }

            decimal profit = DTO.Income - evaluationDetailDTOs.Sum(a => a.Amount.GetValueOrDefault());
            return new Result<EvaluationDTO>
            {
                IsSucceed = true,
                Message = "Details Calculated Successfully",
                Data = new EvaluationDTO
                {
                    Id = DTO.Id,
                    From = from.ToShortDateString(),
                    To = to.ToShortDateString(),
                    Income = DTO.Income,
                    Name = DTO.Name,
                    LastUpdatedAt = DateTime.Now.ToString("g"),
                    Note = DTO.Note,
                    Profit = profit,
                    ProfitPercentage = Math.Round((profit / DTO.Income) * 100, 2),
                    EvaluationDetails = evaluationDetailDTOs.OrderByDescending(d => d.Amount)
                }
            };

        }

        public async Task<ConfirmationResponse> Create(EvaluationDTO DTO, int userId)
        {
            DateTime from, to;

            if (DateTime.TryParse(DTO.From, out DateTime parsedFrom) && DateTime.TryParse(DTO.To, out DateTime parsedTo))
            {
                from = parsedFrom;
                to = parsedTo;
            }
            else
            {
                return new ConfirmationResponse { IsSucceed = false, Message = "Invalid Date Format" };
            }

            if (DTO.EvaluationDetails == null || !DTO.EvaluationDetails.Any() || DTO.EvaluationDetails.Sum(a => a.Amount ?? 0) == 0)
            {
                return new ConfirmationResponse { IsSucceed = false, Message = "No Details Provided" };
            }

            if (DTO.Income <= 0)
                return new ConfirmationResponse { IsSucceed = false, Message = "Income Should Be Grater Than Zero" };

            var newEvaluation = new Evaluation
            {
                Name = DTO.Name,
                From = from,
                To = to,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,
                Income = DTO.Income,
                Profit = DTO.Profit,
                Note = DTO.Note,
                userId = userId,
                EvaluationDetails = DTO.EvaluationDetails.Select(d => new EvaluationDetail
                {
                    AccountId = d.AccountId,
                    Amount = d.Amount.GetValueOrDefault(),
                }).ToList()

            };

            await _uow.Evaluations.AddAsync(newEvaluation);
            await _uow.SaveChangesAync();
            return new ConfirmationResponse { IsSucceed = true, Message = "Evaluation Has Been Created Successfully"  };
        }


        public async Task<ConfirmationResponse> Edit(EvaluationDTO DTO)
        {
            var evaluation = await _uow.Evaluations.Get(p => p.Id == DTO.Id, "EvaluationDetails");

            if (evaluation is null || evaluation.IsDeleted)
                return new ConfirmationResponse { IsSucceed = false, Message = "Evaluation Is Not Exist!" };


            DateTime from, to;

            if (DateTime.TryParse(DTO.From, out DateTime parsedFrom) && DateTime.TryParse(DTO.To, out DateTime parsedTo))
            {
                from = parsedFrom;
                to = parsedTo;
            }
            else
            {
                return new ConfirmationResponse { IsSucceed = false, Message = "Invalid Date Format" };
            }

            if (DTO.EvaluationDetails == null || !DTO.EvaluationDetails.Any() || DTO.EvaluationDetails.Sum(a => a.Amount ?? 0) == 0)
            {
                return new ConfirmationResponse { IsSucceed = false, Message = "No Details Provided" };
            }

            if (DTO.Income <= 0)
                return new ConfirmationResponse { IsSucceed = false, Message = "Income Should Be Grater Than Zero" };

            evaluation.Name = DTO.Name;
            evaluation.From = from;
            evaluation.To = to;
            evaluation.LastUpdatedAt = DateTime.Now;
            evaluation.Income = DTO.Income;
            evaluation.Profit = DTO.Profit;
            evaluation.Note = DTO.Note;

            _uow.EvaluationDetails.DeleteRange(evaluation.EvaluationDetails);
            evaluation.EvaluationDetails.Clear();

            
            if (DTO.EvaluationDetails != null && DTO.EvaluationDetails.Any())
            {
                evaluation.EvaluationDetails = DTO.EvaluationDetails.Select(d => new EvaluationDetail
                {
                    AccountId = d.AccountId,
                    Amount = d.Amount.GetValueOrDefault(),
                }).ToList();
            }


            _uow.Evaluations.Update(evaluation);
            await _uow.SaveChangesAync();
            return new ConfirmationResponse { IsSucceed = true, Message = "Evaluation Has Been Created Successfully" };
        }
        public async Task<ConfirmationResponse> Delete(int id)
        {
            var evaluation = await _uow.Evaluations.Get(p => p.Id == id);

            if (evaluation is null || evaluation.IsDeleted)
                return new ConfirmationResponse { IsSucceed = false, Message = "Evaluation Is Not Exist!" };

            await _uow.EvaluationDetails.ExecuteUpdateAsync(j => j.EvaluationId == id, e => e.SetProperty(j => j.IsDeleted, true));

            evaluation.IsDeleted = true;

            _uow.Evaluations.Update(evaluation);
            await _uow.SaveChangesAync();
            return new ConfirmationResponse { IsSucceed = true, Message = "Evaluation Has Been Deleted Successfully" }; ;
        }

    }
}
