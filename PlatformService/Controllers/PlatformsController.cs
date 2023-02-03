using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/platforms")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
    {
        var platforms = _platformRepository.GetAll();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platforms));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDTO> GetPlatformById(int id)
    {
        var platform = _platformRepository.GetById(id);

        if (platform == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PlatformReadDTO>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDTO>> CreatePlatform([FromBody] PlatformCreateDTO platform)
    {
        var platformModel = _mapper.Map<Platform>(platform);
        _platformRepository.Create(platformModel);
        _platformRepository.SaveChanges();

        var result = _mapper.Map<PlatformReadDTO>(platformModel);

        try
        {
            await _commandDataClient.SendPlatformToCommand(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }

        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDTO>(result);
            platformPublishedDto.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetPlatformById), new { Id = result.Id }, result);
    }
}