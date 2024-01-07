using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class TrackService : ITrackService
    {
        public ITrackRepository _trackRepository { get; }

        public TrackService(ITrackRepository trackRepository)
        {
            _trackRepository = trackRepository;
        }


        public async Task<Result<List<TrackDTO>>> GetAll()
        {
            var track = await _trackRepository.GetAll();
            if (track == null)
            {
                return Result<List<TrackDTO>>.Failure(null, "no tracks found");
            }
            try
            {
                var trackDTOs = new List<TrackDTO>();
                foreach (var s in track)
                {
                    trackDTOs.Add(new TrackDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                    });

                }
                return Result<List<TrackDTO>>.Success(trackDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<TrackDTO>>.Failure(null, $"unable to get trackes{ex.InnerException.Message}");

            }
        }

            public async Task<Result<TrackDTO>> GetById(int id)
        {
            try
            {
                var track = await _trackRepository.GetById(id);
                if (track != null)
                {
                    var trackDTO = new TrackDTO
                    {
                        Id = track.Id,
                        Name = track.Name,
                    };

                    return Result<TrackDTO>.Success(trackDTO);
                }
                else
                {
                    return Result<TrackDTO>.Failure(null, "no tracks found");
                }

            }
            catch (Exception ex)
            {
                return Result<TrackDTO>.Failure(null, $"unable to get track{ex.InnerException.Message}");
            }
        }
    }
}
