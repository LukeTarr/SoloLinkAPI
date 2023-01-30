using Microsoft.AspNetCore.Mvc;
using SoloLinkAPI.DTOs;

namespace SoloLinkAPI.Services;

public interface ILinkService
{
    public Task<IActionResult> Post(LinkDTO dto);
    public Task<IActionResult> Put(int id, LinkDTO dto);
    public Task<IActionResult> Delete(int id);
}