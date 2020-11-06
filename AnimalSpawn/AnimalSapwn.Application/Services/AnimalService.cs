using AnimalSpawn.Domain.Entities;
using AnimalSpawn.Domain.Exceptions;
using AnimalSpawn.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AnimalSapwn.Application.Services
{
	public class AnimalService : IAnimalServices
	{


		private readonly IUnitOfWork _unitOfWork;
		public AnimalService (IUnitOfWork unitOfWork)
		{
			this._unitOfWork = unitOfWork;
		}

		public async Task AddAnimal(Animal animal)
		{
			Expression<Func<Animal, bool>> exprAnimal = item => item.Name == animal.Name;
			var animals = _unitOfWork.AnimalRepository.FindByCondition(exprAnimal);


			if (animals.Any(item => item.Name == animal.Name))
				throw new BusinessExceptions("This animal name already exist.");

			if (animal?.EstimatedAge > 0 && (animal?.Weight <= 0 || animal?.Height <= 0))
				throw new BusinessExceptions("The height and weight should be greater than zero.");

			var older = DateTime.Now - (animal?.CaptureDate ?? DateTime.Now);

			if (older.TotalDays > 45)
				throw new BusinessExceptions("The animal's capture date is older than 45 days");
			Expression<Func<RfidTag, bool>> expressionTag = tag => tag.Tag == animal.RfidTag.Tag;
			if (animal.RfidTag != null)
			{
				Expression<Func<RfidTag, bool>> exprTag = item => item.Tag == animal.RfidTag.Tag;
				var tags = _unitOfWork.RfifTagRepository.FindByCondition(exprTag);
			}

			await _unitOfWork.AnimalRepository.Add(animal);


		}

		public async Task DeleteAnimal(int id)
		{
				await _unitOfWork.AnimalRepository.Delete(id);
		}
		public async Task<Animal> GetAnimal(int id)
		{
			return await _unitOfWork.AnimalRepository.GetById(id);
		}
		public IEnumerable<Animal> GetAnimals()
		{
			return _unitOfWork.AnimalRepository.GetAll();
		}
		public void UpdateAnimal(Animal animal)
		{
			_unitOfWork.AnimalRepository.Update(animal);
			_unitOfWork.SaveChangesAsync();

		}

		
	}

}



