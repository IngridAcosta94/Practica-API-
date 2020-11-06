using AnimalSpawn.Api.Responses;
using AnimalSpawn.Domain.DTOs;
using AnimalSpawn.Domain.Entities;
using AnimalSpawn.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalSpawn.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalServices _service;
        private readonly IMapper _mapper;
        public AnimalController(IAnimalServices service, IMapper mapper)
        {
            _service = service;
            this._mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var animals = _service.GetAnimals();
            var animalsDto = _mapper.Map<IEnumerable<Animal>, IEnumerable<AnimalResponseDto>>(animals);
            var response = new ApiResponse<IEnumerable<AnimalResponseDto>>(animalsDto);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var animal = await _service.GetAnimal(id);
            var animalDto = _mapper.Map<Animal, AnimalResponseDto>(animal);
            var response = new ApiResponse<AnimalResponseDto>(animalDto);
            return Ok(response);

        }

        [HttpPost]
        public async Task<IActionResult> Post(AnimalRequestDto animalDto ,string Correo,string  CorreoCop , string UrlFirma)

        {
            System.Net.Mail.MailMessage mssg = new System.Net.Mail.MailMessage();
            mssg.To.Add(Correo);
            mssg.Subject = "Firma Documento";
            mssg.SubjectEncoding = System.Text.Encoding.UTF8;
            mssg.Bcc.Add(CorreoCop);//para que le llegue copia a alguien
            mssg.Body = "Ingresa a siguiente enlace para firmar " + UrlFirma ;
            mssg.BodyEncoding = System.Text.Encoding.UTF8;
            mssg.IsBodyHtml = true;
            mssg.From = new System.Net.Mail.MailAddress("ingridacosta322@gmail.com");


            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();
            cliente.Credentials = new System.Net.NetworkCredential("ingridacosta322@gmail.com", "ingrid1234");//remitente
            cliente.Port = 587;
            cliente.EnableSsl = true;
            cliente.Host = "smtp.gmail.com";
            try
            {

                cliente.Send(mssg);

               // MessageBox.Show("Cita Cancelada y se envio correo de notificacion.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
               // MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            


            var animal = _mapper.Map<AnimalRequestDto, Animal>(animalDto);
            await _service.AddAnimal(animal);
            var animalresponseDto = _mapper.Map<Animal, AnimalResponseDto>(animal);
            var response = new ApiResponse<AnimalResponseDto>(animalresponseDto);
            return Ok(response);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.DeleteAnimal(id);
            var response = new ApiResponse<bool>(true);
            return Ok(response);

        }
        [HttpPut]
        public async Task<IActionResult> Put(int id, AnimalRequestDto animalDto)
        {
            var animal = _mapper.Map<Animal>(animalDto);
            animal.Id = id;
            animal.UpdateAt = DateTime.Now;
            animal.UpdatedBy = 2;
             _service.UpdateAnimal(animal);
            var response = new ApiResponse<bool>(true);
            return Ok(response);
        }


    }
}
