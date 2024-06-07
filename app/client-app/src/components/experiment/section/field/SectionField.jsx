import { HStack, Heading, Text } from "@chakra-ui/react";
import {
  ChemicalDisposableTable,
  GreenMetricsCalculator,
  GroupPlanTable,
  HazardSummaryTable,
  ProductYieldTable,
  MultiProductYieldTable,
  ReactionScheme,
  MultiReactionScheme,
} from "components/experiment/forms";
import { DraggableListField } from "components/forms/DraggableListField";
import { FileUploadField } from "components/forms/FileUploadField";
import { NumberInputField } from "components/forms/NumberInputField";
import { OptionsField } from "components/forms/OptionsField";
import { TextAreaField } from "components/forms/TextAreaField";
import { TextField } from "components/forms/TextField";
import { ImageUploadField } from "components/forms/imageUploadField/ImageUploadField";
import { INPUT_TYPES } from "constants/input-types";
import { Feedback } from "../feedback/Feedback";

/**
 * Creates a field based on the field type
 * @param {*}
 * field: field object
 * isInstructor: boolean
 * @returns
 */
export const SectionField = ({ field, isDisabled }) => {
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
  } = INPUT_TYPES;

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
          <Heading size="xs" fontWeight="semibold">
            {field.name}
          </Heading>
          <Text fontSize="sm">{field.defaultResponse}</Text>
        </HStack>
      );
    case TextFieldType.toUpperCase():
      return (
        <HStack>
          <TextField
            name={field.id}
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
          <TextField
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
          <DraggableListField name={field.id} label={field.name} />
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
          <MultiReactionScheme name={field.id} isDisabled={isDisabled} />
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
          <MultiProductYieldTable
            name={field.id}
            label={field.name}
            isDisabled={isDisabled}
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

    default:
      return null;
  }
};
