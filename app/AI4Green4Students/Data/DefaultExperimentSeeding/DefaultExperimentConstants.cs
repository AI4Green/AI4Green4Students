namespace AI4Green4Students.Data.DefaultExperimentSeeding;

/// <summary>
/// Placeholder constants for the default experiment seeder. This is an optional project which is created for the first draft of
/// AI4Green4Students, as well as an example to quickly setup a default project, complete with fields.
/// </summary>
public static class DefaultExperimentConstants
{
  // Section names for the Project Group Summary (only one section)
  public static string ProjectGroupSummarySection = "Project Group Summary";
  
  // Section names for the Literature Review (only one section)
  public static string LiteratureReviewSection = "Literature Review";
  
  // Section names for the Plan
  public static string ReactionSchemeSection = "Reaction Scheme"; // for both the plan and the note
  public static string CoshhSection = "COSHH Form";
  public static string SafetyDataSection = "Safety Data";
  public static string ExperimentalProcecureSection = "Experimental Procedure";
  
  // Section names for the Note
  public static string MetadataSection = "Metadata";
  public static string YieldAndGreenMetricsCalcSection = "Calculate Yield and Green Metrics";
  public static string ReactionDescriptionSection = "Reaction Description";
  public static string WorkupDescriptionSection = "Workup Description";
  public static string TLCAnalysisSection = "TLC Analysis";
  public static string ProductCharacterisatonSection = "Characterisation of Product";
  public static string ObeservationAndInferencesSection = "Observations and Inferences";
  
  public static string ProjectTitleField = "Project Title";
  
  // Field names for the Project Group Summary section
  public static string PGGroupPlanField = "Project group plan";
  public static string PGHazardSummaryField = "Project group hazard summary";
  public static string PGLiteratureSummaryField = "Project group literature summary";
  public static string PGNotes = "Notes/methods";
  
  // Field names for the Literature Review section
  public static string LiteratureReviewTextField = "Literature Review Summary";
  public static string LiteratureReviewFileUpload = "Attach paper-full text";
  
  // Field names for the Plan sections
  public static string ReactionSchemeField = "Draw Structure";

  // COSHH Form section fields (one of the sections in the Plan)
  public static string SafetyRiskImplicationsField = "Safety and Risk Implications (select as appropriate)";
  public static string FireRiskField = "Fire or Explosion Risk";
  public static string FireRiskPreventionField = "Fire or Explosion Risk Prevention";
  public static string ThermalField = "Thermal Runaway or Gas Release";
  public static string ThermalPreventionField = "Thermal Runaway or Gas Release Prevention";
  public static string MalodorousField = "Malodorous Substances";
  public static string MalodorousPreventionField = "Malodorous Substances Prevention";
  public static string AdditionalSafetyField = "Additional Safety Implications (tick as appropriate)";
  public static string ControlMeasuresField = "Control Measures";
  public static string FumehoodFieldOption = "Fumehood";
  public static string RubberGlovesFieldOption = "Rubber Gloves";
  public static string DustMaskFieldOption = "Dust Mask";
  public static string HeavyGlovesFieldOption = "Heavy Gloves";
  public static string InertAtmosFieldOption = "Inert Atmosphere";
  public static string EyeProtectionFieldOption = "Eye Protection";
  public static string NitrileGlovesFieldOption = "Nitrile Gloves";
  public static string ScreensFieldOption = "Screens";
  public static string SpillageTrayFieldOption = "SpillageTray";
  public static string PrimaryContainmentField = "Primary Containment";
  public static string OpenContainmentFieldOption = "Open Containment";
  public static string FlaskCondensorFieldOption = "Flask and Condenser";
  public static string MultiNeckFieldOption = "Multi-Neck Flask and Condenser";
  public static string SealedFlaskFieldOption = "Sealed Flask / tube";
  public static string OtherRisksField = "Other Risks and Control Measures (please specify)";
  public static string EmergencyProceduresField = "Emergency Procedures (please specify)";
  public static string RiskCategoryField = "Risk Category (tick as appropriate)";
  public static string AFieldOption = "A";
  public static string BFieldOption = "B"; 
  public static string CFieldOption = "C"; 
  public static string DFieldOption = "D";
  public static string WasteDisposalField = "Waste disposal (specify for each chemical)";
  public static string AdditionalControlsField = "Additional Controls and Special Procedures";
  public static string EmergencyProceduresMultiField = "Emergency Procedures (tick as appropriate)";
  public static string Co2FireExtinguisherFieldOption = "CO2 Fire Extinguisher";
  public static string DryPowderFireExtinguisherFieldOption = "Dry Powder Fire Extinguisher";
  public static string SpillKitFieldOption = "Spill Kit";
  public static string EvacuateAreaFieldOption = "Evacuate Area";
  public static string WashDownAreaFieldOption = "Wash Down Area";
  public static string EmergencyTreatmentField = "Emergency Treatment in case of Contamination or Exposure";
  public static string ECStandardProtocolField = "Exposure/Contamination-standard protocol as detailed below (Special procedures MUST also be specified below)";
  public static string MESExposureField = "Mouth, Eyes, Skin exposure";
  public static string MESExposureFieldContent = "Flush area of contact with plenty of water, contact a First Aider;";
  public static string LungsField = "Lungs";
  public static string LungsFieldContent = "Remove to fresh air, contact a first aider.";
  public static string IfSwallowedField = "If Swallowed";
  public static string IfSwallowedFieldContent = "Contact a first aider, get details of substance ingested and seek medical attention immediately.";
  public static string IfUnconsciousField = "If Unconscious";
  public static string IfUnconsciousFieldContent = "Contact a first aider immediately and call an ambulance.";

  public static string SafetyDataField = "Safety Data from literature (including toxicity)";
  public static string ExperimentalProcedureField = "Experimental Procedure - materials and steps";
  
  // Field names for the Note sections
  
  // Metadata section fields
  public static string ReactionNameField = "Reaction Name";
  public static string StatusField = "Status";
  public static string TemperatureField = "Temperature";
  public static string StartDateAndTimeField = "Start Date and Time";
  public static string EndDateAndTimeField = "End Date and Time";
  public static string DurationField = "Duration";
  
  // Calculate Yield and Green Metrics section fields
  public static string YieldCalculationField = "Calculate Yield";
  public static string GreenMetricsCalculationField = "Calculate Green Metrics";
  
  public static string ReactionDescriptionField = "Reaction Description";
  
  public static string WorkupDescriptionField = "Workup Description";
  
  public static string TLCAnalysisField = "TLC Analysis";
  public static string TLCAnalysisImgUploadField = "TLC Images";
  
  public static string ProductCharacterisationField = "Characterisation of Product";
  public static string ProductCharImgUploadField = "TLC Images";
  
  public static string ObeservationAndInferencesField = "Observations and Inferences";
}
