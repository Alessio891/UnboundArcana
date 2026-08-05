using System.Collections.Generic;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Spells.Data
{
	public enum SpellConfigurationSlot
	{
		Behavior,
		Principle,
		CatalystA,
		CatalystB,
		Flux
	}

	public enum SpellConfigurationValidationError
	{
		None,
		MissingBehavior,
		InvalidPrincipleCategory,
		IncompatiblePrinciple,
		InvalidCatalystACategory,
		IncompatibleCatalystA,
		InvalidCatalystBCategory,
		IncompatibleCatalystB,
		InvalidFluxCategory,
		IncompatibleFlux,
		DuplicateModule
	}

	public readonly struct SpellConfigurationValidationResult
	{
		public SpellConfigurationValidationError Error { get; }
		public string Reason { get; }
		public bool IsValid => Error == SpellConfigurationValidationError.None;

		public SpellConfigurationValidationResult(SpellConfigurationValidationError error, string reason)
		{
			Error = error;
			Reason = reason;
		}
	}

	public class SpellConfiguration
	{
		private SpellBehaviorDefinition behavior;
		private SpellModuleDefinition principle;
		private SpellModuleDefinition catalystA;
		private SpellModuleDefinition catalystB;
		private SpellModuleDefinition flux;

		public float Cooldown { get; }
		public SpellBehaviorDefinition Behavior => behavior;
		public SpellModuleDefinition Principle => principle;
		public SpellModuleDefinition CatalystA => catalystA;
		public SpellModuleDefinition CatalystB => catalystB;
		public SpellModuleDefinition Flux => flux;
		public int ModuleCount => CountModules();
		public SpellConfiguration(
			SpellDefinition definition)
		{
			if (definition == null)
			{
				return;
			}

			behavior = definition.behavior;
			principle = definition.principle;
			catalystA = definition.catalystA;
			catalystB = definition.catalystB;
			flux = definition.flux;
			Cooldown = definition.cooldown;
		}

		public IEnumerable<SpellModuleDefinition> Modules
		{
			get
			{
				if (principle != null) { yield return principle; }
				if (catalystA != null) { yield return catalystA; }
				if (catalystB != null) { yield return catalystB; }
				if (flux != null) { yield return flux; }
			}
		}

		public SpellModuleDefinition GetModule(SpellConfigurationSlot slot)
		{
			switch (slot)
			{
				case SpellConfigurationSlot.Principle:
					return principle;
				case SpellConfigurationSlot.CatalystA:
					return catalystA;
				case SpellConfigurationSlot.CatalystB:
					return catalystB;
				case SpellConfigurationSlot.Flux:
					return flux;
				default:
					return null;
			}
		}

		public bool HasModule(SpellModuleDefinition module)
		{
			return module != null && (principle == module || catalystA == module || catalystB == module || flux == module);
		}

		public bool TryGetSlot(SpellModuleDefinition module, out SpellConfigurationSlot slot)
		{
			if (module != null)
			{
				if (principle == module) { slot = SpellConfigurationSlot.Principle; return true; }
				if (catalystA == module) { slot = SpellConfigurationSlot.CatalystA; return true; }
				if (catalystB == module) { slot = SpellConfigurationSlot.CatalystB; return true; }
				if (flux == module) { slot = SpellConfigurationSlot.Flux; return true; }
			}

			slot = SpellConfigurationSlot.Behavior;
			return false;
		}

		public bool TryGetAvailableSlot(SpellModuleType type, out SpellConfigurationSlot slot)
		{
			switch (type)
			{
				case SpellModuleType.Principle:
					if (principle == null) { slot = SpellConfigurationSlot.Principle; return true; }
					break;
				case SpellModuleType.Catalyst:
					if (catalystA == null) { slot = SpellConfigurationSlot.CatalystA; return true; }
					if (catalystB == null) { slot = SpellConfigurationSlot.CatalystB; return true; }
					break;
				case SpellModuleType.Flux:
					if (flux == null) { slot = SpellConfigurationSlot.Flux; return true; }
					break;
			}

			slot = SpellConfigurationSlot.Behavior;
			return false;
		}

		public SpellConfigurationValidationResult Validate()
		{
			if (behavior == null) { return Invalid(SpellConfigurationValidationError.MissingBehavior, "Behavior slot is empty."); }
			if (principle != null)
			{
				if (principle.Type != SpellModuleType.Principle) { return Invalid(SpellConfigurationValidationError.InvalidPrincipleCategory, "Principle slot requires a Principle module."); }
				if (!principle.CanAddTo(this)) { return Invalid(SpellConfigurationValidationError.IncompatiblePrinciple, "The Principle module is not compatible with the selected spell configuration."); }
			}
			if (catalystA != null)
			{
				if (catalystA.Type != SpellModuleType.Catalyst) { return Invalid(SpellConfigurationValidationError.InvalidCatalystACategory, "Catalyst A slot requires a Catalyst module."); }
				if (!catalystA.CanAddTo(this)) { return Invalid(SpellConfigurationValidationError.IncompatibleCatalystA, "Catalyst A is not compatible with the selected spell configuration."); }
			}
			if (catalystB != null)
			{
				if (catalystB.Type != SpellModuleType.Catalyst) { return Invalid(SpellConfigurationValidationError.InvalidCatalystBCategory, "Catalyst B slot requires a Catalyst module."); }
				if (!catalystB.CanAddTo(this)) { return Invalid(SpellConfigurationValidationError.IncompatibleCatalystB, "Catalyst B is not compatible with the selected spell configuration."); }
			}
			if (flux != null)
			{
				if (flux.Type != SpellModuleType.Flux) { return Invalid(SpellConfigurationValidationError.InvalidFluxCategory, "Flux slot requires a Flux module."); }
				if (!flux.CanAddTo(this)) { return Invalid(SpellConfigurationValidationError.IncompatibleFlux, "Flux is not compatible with the selected spell configuration."); }
			}
			if ((principle != null && (principle == catalystA || principle == catalystB || principle == flux)) ||
				(catalystA != null && (catalystA == catalystB || catalystA == flux)) ||
				(catalystB != null && catalystB == flux))
			{
				return Invalid(SpellConfigurationValidationError.DuplicateModule, "A module cannot occupy more than one spell slot.");
			}

			return new SpellConfigurationValidationResult(SpellConfigurationValidationError.None, string.Empty);
		}

		internal void SetBehavior(SpellBehaviorDefinition value)
		{
			behavior = value;
		}

		internal bool SetModule(SpellConfigurationSlot slot, SpellModuleDefinition module)
		{
			switch (slot)
			{
				case SpellConfigurationSlot.Principle:
					principle = module;
					return true;
				case SpellConfigurationSlot.CatalystA:
					catalystA = module;
					return true;
				case SpellConfigurationSlot.CatalystB:
					catalystB = module;
					return true;
				case SpellConfigurationSlot.Flux:
					flux = module;
					return true;
				default:
					return false;
			}
		}

		private int CountModules()
		{
			int count = 0;
			if (principle != null) { count++; }
			if (catalystA != null) { count++; }
			if (catalystB != null) { count++; }
			if (flux != null) { count++; }
			return count;
		}

		private SpellConfigurationValidationResult Invalid(SpellConfigurationValidationError error, string reason)
		{
			return new SpellConfigurationValidationResult(error, reason);
		}
	}
}
