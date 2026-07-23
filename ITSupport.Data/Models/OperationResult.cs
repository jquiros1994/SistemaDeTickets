using System.Collections.Generic;

namespace ITSupport.Models
{
	// SOLID: SRP - esta clase tiene una unica responsabilidad: representar el resultado
	// (exito o error) de una operacion
	// DP: Result Object 
	public class OperationResult
	{
		public bool Success { get; private set; }
		public int RecordId { get; set; }
		public List<string> Errors { get; private set; }

		private OperationResult()
		{
			Errors = new List<string>();
		}

		// DP: Factory Method - metodos estaticos que centralizan la creacion de instancias
		// validas del objeto, evitando estados inconsistentes.
		public static OperationResult Ok(int recordId = 0)
		{
			return new OperationResult { Success = true, RecordId = recordId };
		}

		public static OperationResult Fail(params string[] errors)
		{
			var result = new OperationResult { Success = false };
			result.Errors.AddRange(errors);
			return result;
		}

		public static OperationResult Fail(List<string> errors)
		{
			var result = new OperationResult { Success = false };
			result.Errors.AddRange(errors);
			return result;
		}
	}
}
