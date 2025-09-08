import { Heading, HStack, Text } from "@chakra-ui/react";
import {
  DraggableListField,
  FileUploadField,
  FormattedTextInput,
  FormikInput,
  ImageUploadField,
  NumberInputField,
  OptionsField,
  TextAreaField,
} from "components/core/forms";
import {
  ChemicalDisposableTable,
  GreenMetricsCalculator,
  GroupPlanTable,
  HazardSummaryTable,
  ProductYieldTable,
  ReactionScheme,
  TabbedImportPanel,
} from "components/experiment-forms";
import { Feedback } from "components/feedback/feedback";
import { FIELDS, INPUT_TYPES, SECTION_TYPES } from "constants";

const FieldWrapper = ({ hasFeedback = true, field, item, children }) => (
  <HStack>
    {children}
    {hasFeedback && <Feedback field={field} item={item} />}
  </HStack>
);

const Header = ({ field }) => (
  <Heading size="sm" as="u">
    {field.name}
  </Heading>
);

const Content = ({ field }) => (
  <HStack>
    <Text fontSize="sm" fontWeight="semibold">
      {field.name}
    </Text>
    <Text fontSize="sm">{field.defaultResponse}</Text>
  </HStack>
);

const TextField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <FormikInput
      name={field.id}
      label={field.name}
      isRequired={field.mandatory}
      placeholder={field.name}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const FormattedTextField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <FormattedTextInput
      name={field.id}
      label={field.name}
      isRequired={field.mandatory}
      placeholder={field.name}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const DateAndTimeField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <FormikInput
      name={field.id}
      label={field.name}
      isRequired={field.mandatory}
      placeholder={field.name}
      isDisabled={isDisabled}
      type="datetime-local"
    />
  </FieldWrapper>
);

const NumberField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <NumberInputField
      name={field.id}
      label={field.name}
      isRequired={field.mandatory}
      placeholder={field.name}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const DescriptionField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <TextAreaField
      name={field.id}
      title={field.name}
      placeholder={field.name}
      isRequired={field.mandatory}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const FileField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <FileUploadField
      name={field.id}
      title={field.name}
      accept={field.fieldResponse?.accept ?? [".pdf", ".docx", ".doc"]}
      isRequired={field.mandatory}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const ImageFileField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <ImageUploadField
      name={field.id}
      title={field.name}
      accept={field.fieldResponse?.accept ?? [".png", ".jpg", ".jpeg"]}
      isRequired={field.mandatory}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const SortableListField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <DraggableListField
      name={field.id}
      label={field.name}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const ReactionSchemeField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <ReactionScheme name={field.id} isDisabled={isDisabled} />
  </FieldWrapper>
);

const MultiReactionSchemeField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <TabbedImportPanel
      name={field.id}
      label={field.name}
      isDisabled={isDisabled}
      sourceType={SECTION_TYPES.Note}
      fieldName={FIELDS.ReactionSchemeField}
      Component={ReactionScheme}
    />
  </FieldWrapper>
);

const ChemicalDisposalTableField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <ChemicalDisposableTable
      name={field.id}
      label={field.name}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const MultipleField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <OptionsField
      name={field.id}
      label={field.name}
      options={field.selectFieldOptions}
      isMultiple
      isDisabled={isDisabled}
      isRequired={field.mandatory}
    />
  </FieldWrapper>
);

const RadioField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <OptionsField
      name={field.id}
      label={field.name}
      options={field.selectFieldOptions}
      isDisabled={isDisabled}
      isRequired={field.mandatory}
    />
  </FieldWrapper>
);

const ProjectGroupPlanTableField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <GroupPlanTable
      name={field.id}
      label={field.name}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const ProjectGroupHazardTableField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <HazardSummaryTable
      name={field.id}
      label={field.name}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const YieldTableField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <ProductYieldTable
      name={field.id}
      label={field.name}
      isDisabled={isDisabled}
    />
  </FieldWrapper>
);

const MultiYieldTableField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <TabbedImportPanel
      name={field.id}
      label={field.name}
      isDisabled={isDisabled}
      sourceType={SECTION_TYPES.Note}
      fieldName={FIELDS.YieldCalculationField}
      Component={ProductYieldTable}
    />
  </FieldWrapper>
);

const GreenMetricsTableField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <GreenMetricsCalculator name={field.id} isDisabled={isDisabled} />
  </FieldWrapper>
);

const MultiGreenMetricsTableField = ({ field, isDisabled, item }) => (
  <FieldWrapper field={field} item={item}>
    <TabbedImportPanel
      name={field.id}
      label={field.name}
      isDisabled={isDisabled}
      sourceType={SECTION_TYPES.Note}
      fieldName={FIELDS.GreenMetricsTable}
      Component={GreenMetricsCalculator}
    />
  </FieldWrapper>
);

const INPUT_TYPES_MAP = {
  [INPUT_TYPES.Header]: Header,
  [INPUT_TYPES.Content]: Content,
  [INPUT_TYPES.TextFieldType]: TextField,
  [INPUT_TYPES.FormattedTextFieldType]: FormattedTextField,
  [INPUT_TYPES.DateAndTime]: DateAndTimeField,
  [INPUT_TYPES.Number]: NumberField,
  [INPUT_TYPES.Description]: DescriptionField,
  [INPUT_TYPES.File]: FileField,
  [INPUT_TYPES.ImageFile]: ImageFileField,
  [INPUT_TYPES.SortableList]: SortableListField,
  [INPUT_TYPES.ExperimentReactionScheme]: ReactionSchemeField,
  [INPUT_TYPES.ExperimentMultiReactionScheme]: MultiReactionSchemeField,
  [INPUT_TYPES.ChemicalDisposalTable]: ChemicalDisposalTableField,
  [INPUT_TYPES.Multiple]: MultipleField,
  [INPUT_TYPES.Radio]: RadioField,
  [INPUT_TYPES.ProjectGroupPlanTable]: ProjectGroupPlanTableField,
  [INPUT_TYPES.ProjectGroupHazardTable]: ProjectGroupHazardTableField,
  [INPUT_TYPES.YieldTable]: YieldTableField,
  [INPUT_TYPES.MultiYieldTable]: MultiYieldTableField,
  [INPUT_TYPES.GreenMetricsTable]: GreenMetricsTableField,
  [INPUT_TYPES.MultiGreenMetricsTable]: MultiGreenMetricsTableField,
};

/**
 * Creates a field based on its input type.
 */
export const Field = ({ field, isDisabled, item }) => {
  const inputType = field.inputType.name.toUpperCase();
  const [, Component] =
    Object.entries(INPUT_TYPES_MAP).find(
      ([key]) => key.toUpperCase() === inputType
    ) || [];

  if (!Component) {
    return null;
  }
  return <Component field={field} isDisabled={isDisabled} item={item} />;
};
