﻿using Infrastructure.Services;

namespace Application.IServices;
public interface IHomeService
{
    public Task<GetHomeDTO> GetHome();
}