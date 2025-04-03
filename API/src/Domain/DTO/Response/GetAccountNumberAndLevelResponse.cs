using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Response;
public record GetAccountNumberAndLevelResponse : ConfirmationResponse
{
    public int AccountLevel { get; set; } = 1;
    public string AccountNumber { get; set; } = string.Empty;
}
