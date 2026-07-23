using System;
using System.Collections.Generic;
using ITSupport.DAL;
using ITSupport.Models;

namespace ITSupport.Business
{
	// SOLID: SRP 
	public class CaseBusiness
	{
		// SOLID: DIP 
		private readonly ICaseRepository _repository;
		private readonly ICaseStatusRepository _statuses;
		private readonly ICatalogRepository<Priority> _priorities;
		private readonly ICatalogRepository<Contact> _contacts;
		private readonly ICatalogRepository<SupportEngineer> _engineers;
		private readonly ICatalogRepository<SupportProgram> _programs;

		// DP: 
		public CaseBusiness(ICaseRepository repository,
							ICaseStatusRepository statuses,
							ICatalogRepository<Priority> priorities,
							ICatalogRepository<Contact> contacts,
							ICatalogRepository<SupportEngineer> engineers,
							ICatalogRepository<SupportProgram> programs)
		{
			_repository = repository;
			_statuses = statuses;
			_priorities = priorities;
			_contacts = contacts;
			_engineers = engineers;
			_programs = programs;
		}

		public CaseBusiness() : this(new CaseRepository(),
									 new CaseStatusRepository(),
									 new PriorityRepository(),
									 new ContactRepository(),
									 new SupportEngineerRepository(),
									 new ProgramRepository())
		{
		}


		public List<CaseViewModel> GetAllWithDetails(int? statusId = null, string search = null)
		{
			return _repository.GetAllWithDetails(statusId, search);
		}

		public CaseViewModel GetByIdWithDetails(int caseId)
		{
			return _repository.GetByIdWithDetails(caseId);
		}

		public Case GetById(int caseId)
		{
			return _repository.GetById(caseId);
		}


		public OperationResult Create(Case model)
		{
			var errors = Validate(model);
			if (errors.Count > 0)
				return OperationResult.Fail(errors);

			if (model.StatusId == 0) model.StatusId = 1;

			int newId = _repository.Insert(model);
			return OperationResult.Ok(newId);
		}

		public OperationResult Update(Case model)
		{
			var errors = Validate(model);
			if (errors.Count > 0)
				return OperationResult.Fail(errors);

			var existing = _repository.GetById(model.CaseId);
			if (existing == null)
				return OperationResult.Fail("El caso no existe.");

			if (!_repository.Update(model))
				return OperationResult.Fail("No fue posible actualizar el caso.");

			// SOLID: OCP - cierre/reapertura 
			var newStatus = _statuses.GetById(model.StatusId);
			if (newStatus != null)
			{
				if (!newStatus.IsOpen && existing.ClosedAt == null)
					_repository.SetClosedAt(model.CaseId, DateTime.Now);
				else if (newStatus.IsOpen && existing.ClosedAt != null)
					_repository.SetClosedAt(model.CaseId, null);
			}

			return OperationResult.Ok(model.CaseId);
		}

		// estado cerrado; un caso abierto no puede borrarse.
		public OperationResult Delete(int caseId)
		{
			var existing = _repository.GetById(caseId);
			if (existing == null)
				return OperationResult.Fail("El caso no existe.");

			var status = _statuses.GetById(existing.StatusId);
			if (status != null && status.IsOpen)
				return OperationResult.Fail("No se puede eliminar un caso abierto. Cierrelo o cancelelo primero.");

			return _repository.Delete(caseId)
				? OperationResult.Ok(caseId)
				: OperationResult.Fail("No fue posible eliminar el caso.");
		}

		// SOLID: SRP 
		// Create y Update.
		private List<string> Validate(Case model)
		{
			var errors = new List<string>();

			if (model == null)
			{
				errors.Add("No se recibieron datos del caso.");
				return errors;
			}

			if (string.IsNullOrWhiteSpace(model.Title) || model.Title.Trim().Length < 5)
				errors.Add("El titulo es obligatorio y debe tener al menos 5 caracteres.");

			if (string.IsNullOrWhiteSpace(model.Description) || model.Description.Trim().Length < 10)
				errors.Add("La descripcion es obligatoria y debe tener al menos 10 caracteres.");

			if (model.PriorityId <= 0) errors.Add("Debe seleccionar una prioridad.");
			if (model.ContactId <= 0) errors.Add("Debe seleccionar un contacto.");
			if (model.ProgramId <= 0) errors.Add("Debe seleccionar un programa.");

			if (string.IsNullOrWhiteSpace(model.Country))
				errors.Add("El pais es obligatorio.");

			return errors;
		}

		// ------------------------- Catalogos para las vistas -------------------------

		public List<CaseStatus> GetStatuses()
		{
			return _statuses.GetAll();
		}

		public List<Priority> GetPriorities()
		{
			return _priorities.GetAll();
		}

		public List<Contact> GetContacts()
		{
			return _contacts.GetAll();
		}

		public List<SupportEngineer> GetEngineers()
		{
			return _engineers.GetAll();
		}

		public List<SupportProgram> GetPrograms()
		{
			return _programs.GetAll();
		}
	}
}
