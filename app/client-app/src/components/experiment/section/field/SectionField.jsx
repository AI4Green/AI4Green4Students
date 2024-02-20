import { HStack, Heading, Text } from "@chakra-ui/react";
import { TextField } from "components/forms/TextField";
import { Feedback } from "../feedback/Feedback";
import { TextAreaField } from "components/forms/TextAreaField";
import { FileUploadField } from "components/forms/FileUploadField";
import { DraggableListField } from "components/forms/DraggableListField";
import { ReactionScheme } from "components/experiment/reactionScheme/ReactionScheme";
import { ChemicalDisposableTable } from "components/experiment/chemicalDisposable/ChemicalDisposableTable";
import { OptionsField } from "components/forms/OptionsField";
import { INPUT_TYPES } from "constants/input-types";
import { GroupPlanTable } from "components/experiment/projectGroupActivities/groupPlan/GroupPlanTable";
import { HazardSummaryTable } from "components/experiment/projectGroupActivities/hazardSummary/HazardSummaryTable";

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
    Description,
    File,
    DraggableList,
    ReactionScheme: ExperimentReactionScheme,
    ChemicalDisposalTable,
    Multiple,
    Radio,
    ProjectGroupPlanTable,
    ProjectGroupHazardTable,
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
            isFilePresentName={`${field.id}_isFilePresent`}
            title={field.name}
            accept={field.fieldResponse?.accept ?? [".pdf", ".docx", ".doc"]} // default accepted file ext. is pdf, docx, doc
            existingFile={field.fieldResponse?.fileName}
            downloadLink={field.fieldResponse?.fileName} // TODO: change this to the actual download link
            isRequired={field.mandatory}
            isDisabled={isDisabled}
          />
          <Feedback field={field} />
        </HStack>
      );
    case DraggableList.toUpperCase():
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

    default:
      return null;
  }
};
