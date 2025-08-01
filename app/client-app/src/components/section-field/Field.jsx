import { HStack, Heading, Text } from "@chakra-ui/react";
import {
  ChemicalDisposableTable,
  GreenMetricsCalculator,
  GroupPlanTable,
  HazardSummaryTable,
  ProductYieldTable,
  ReactionScheme,
  TabbedImportPanel,
} from "components/experiment-forms";
import {
  DraggableListField,
  FileUploadField,
  NumberInputField,
  OptionsField,
  TextAreaField,
  FormikInput,
  ImageUploadField,
  FormattedTextInput,
} from "components/core/forms";
import { INPUT_TYPES, SECTION_TYPES, FIELDS } from "constants";
import { Feedback } from "components/feedback/Feedback";

/**
 * Creates a field based on the field type
 * @param {*}
 * field: field object
 * isInstructor: boolean
 * @returns
 */
export const Field = ({ field, isDisabled }) => {
  const {
    Header,
    Content,
    Text: TextFieldType,
    DateAndTime,
    Number,
    Description,
    File,
    ImageFile,
    SortableList,
    ReactionScheme: ExperimentReactionScheme,
    MultiReactionScheme: ExperimentMultiReactionScheme,
    ChemicalDisposalTable,
    Multiple,
    Radio,
    ProjectGroupPlanTable,
    ProjectGroupHazardTable,
    YieldTable,
    MultiYieldTable,
    GreenMetricsTable,
    MultiGreenMetricsTable,
    FormattedTextInput: FormattedTextFieldType, // Added formatted text input type
  } = INPUT_TYPES;

  const { Note } = SECTION_TYPES;

  switch (field.fieldType.toUpperCase()) {
    case Header.toUpperCase():
      return (
        <Heading size="sm" as="u">
          {field.name}
        </Heading>
      );
    case Content.toUpperCase():
      return (
        <HStack>
          <Text fontSize="sm" fontWeight="semibold">
            {field.name}
          </Text>
          <Text fontSize="sm">{field.defaultResponse}</Text>
        </HStack>
      );
    case TextFieldType.toUpperCase():
      return (
        <HStack>
          <FormikInput
            name={field.id}
            label={field.name}
            isRequired={field.mandatory}
            placeholder={field.name}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );
    case FormattedTextFieldType.toUpperCase():
      return (
        <HStack>
          <FormattedTextInput
            name={String(field.id)}
            label={field.name}
            isRequired={field.mandatory}
            placeholder={field.name}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );
    case DateAndTime.toUpperCase():
      return (
        <HStack>
          <FormikInput
            name={field.id}
            label={field.name}
            isRequired={field.mandatory}
            placeholder={field.name}
            isDisabled={isDisabled}
            type="datetime-local"
          />
          <Feedback field={field} />
        </HStack>
      );
    case Number.toUpperCase():
      return (
        <HStack>
          <NumberInputField
            name={field.id}
            label={field.name}
            isRequired={field.mandatory}
            placeholder={field.name}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );
    case Description.toUpperCase():
      return (
        <HStack>
          <TextAreaField
            name={field.id}
            title={field.name}
            placeholder={field.name}
            isRequired={field.mandatory}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );
    case File.toUpperCase():
      return (
        <HStack>
          <FileUploadField
            name={field.id}
            title={field.name}
            accept={field.fieldResponse?.accept ?? [".pdf", ".docx", ".doc"]} // default accepted file ext. is pdf, docx, doc
            isRequired={field.mandatory}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );
    case ImageFile.toUpperCase():
      return (
        <HStack>
          <ImageUploadField
            name={field.id}
            title={field.name}
            accept={field.fieldResponse?.accept ?? [".png", ".jpg", ".jpeg"]} // default accepted file ext. is png, jpg, jpeg
            isRequired={field.mandatory}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );
    case SortableList.toUpperCase():
      return (
        <HStack>
          <DraggableListField
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );

    case ExperimentReactionScheme.toUpperCase():
      return (
        <HStack>
          <ReactionScheme name={field.id} isDisabled={isDisabled} />
          <Feedback field={field} />
        </HStack>
      );

    case ExperimentMultiReactionScheme.toUpperCase():
      return (
        <HStack>
          <TabbedImportPanel
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
            sourceType={Note}
            fieldName={FIELDS.ReactionSchemeField}
            Component={ReactionScheme}
          />

          <Feedback field={field} />
        </HStack>
      );

    case ChemicalDisposalTable.toUpperCase():
      return (
        <HStack>
          <ChemicalDisposableTable
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );

    case Multiple.toUpperCase():
      return (
        <HStack>
          <OptionsField
            name={field.id}
            label={field.name}
            options={field?.selectFieldOptions}
            isMultiple
            isDisabled={isDisabled}
            isRequired={field.mandatory}
          />
          <Feedback field={field} />
        </HStack>
      );

    case Radio.toUpperCase():
      return (
        <HStack>
          <OptionsField
            name={field.id}
            label={field.name}
            options={field?.selectFieldOptions}
            isDisabled={isDisabled}
            isRequired={field.mandatory}
          />
          <Feedback field={field} />
        </HStack>
      );

    case ProjectGroupPlanTable.toUpperCase():
      return (
        <HStack>
          <GroupPlanTable
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
          />
        </HStack>
      );

    case ProjectGroupHazardTable.toUpperCase():
      return (
        <HStack>
          <HazardSummaryTable
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
          />
        </HStack>
      );

    case YieldTable.toUpperCase():
      return (
        <HStack>
          <ProductYieldTable
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );

    case MultiYieldTable.toUpperCase():
      return (
        <HStack>
          <TabbedImportPanel
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
            sourceType={Note}
            fieldName={FIELDS.YieldCalculationField}
            Component={ProductYieldTable}
          />
          <Feedback field={field} />
        </HStack>
      );

    case GreenMetricsTable.toUpperCase():
      return (
        <HStack>
          <GreenMetricsCalculator name={field.id} isDisabled={isDisabled} />
          <Feedback field={field} />
        </HStack>
      );

    case MultiGreenMetricsTable.toUpperCase():
      return (
        <HStack>
          <TabbedImportPanel
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
            sourceType={Note}
            fieldName={FIELDS.GreenMetricsCalculationField}
            Component={GreenMetricsCalculator}
          />
          <Feedback field={field} />
        </HStack>
      );

    default:
      return null;
  }
};
