import { Heading, HStack } from "@chakra-ui/react";
import { TextField } from "components/forms/TextField";
import { TextAreaField } from "components/forms/TextAreaField";
import { FileUploadField } from "components/forms/FileUploadField";
import { fetchKeys } from "api/experiments";
import { DraggableListField } from "components/forms/DraggableListField";
import { INPUT_TYPES } from "constants/input-types";
import { OptionsField } from "components/forms/OptionsField";
import { Feedback } from "./Feedback";
import { useField } from "api/fields";

export const ExperimentField = ({ field, experimentId, isInstructor }) => {
  const { downloadFile } = fetchKeys;
  switch (field.fieldType.toUpperCase()) {
    case INPUT_TYPES.Header.toUpperCase():
      return (
        <Heading size="sm" as="u">
          {field.name}
        </Heading>
      );
    case INPUT_TYPES.Text.toUpperCase():
      return (
        <HStack>
          <TextField
            name={field.id}
            label={field.name}
            isRequired={field.mandatory}
            placeholder={field.name}
            isDisabled={isInstructor || field.isApproved}
          />
          <Feedback field={field} isInstructor={isInstructor} />
        </HStack>
      );
    case INPUT_TYPES.Description.toUpperCase():
      return (
        <HStack>
          <TextAreaField
            name={field.id}
            title={field.name}
            placeholder={field.name}
            isRequired={field.mandatory}
            isDisabled={isInstructor || field.isApproved}
          />
          <Feedback field={field} isInstructor={isInstructor} />
        </HStack>
      );
    case INPUT_TYPES.File.toUpperCase():
      return (
        <HStack>
          <FileUploadField
            name={field.id}
            isFilePresentName={`is${field.name}FilePresent`}
            title={field.name}
            accept={field.fieldResponse?.accept ?? [".pdf", ".docx", ".doc"]} // default accepted file ext. is pdf, docx, doc
            existingFile={field.fieldResponse?.fileName}
            downloadLink={downloadFile(
              experimentId,
              field.fieldResponse?.fileName
            )}
            isRequired={field.mandatory}
            isDisabled={isInstructor || field.isApproved}
          />
          <Feedback field={field} isInstructor={isInstructor} />
        </HStack>
      );
    case INPUT_TYPES.DraggableList.toUpperCase():
      return (
        <HStack>
          <DraggableListField name={field.id} label={field.name} />
          <Feedback field={field} isInstructor={isInstructor} />
        </HStack>
      );

    case INPUT_TYPES.Multiple.toUpperCase(): {
      const CheckBoxField = () => {
        const { data: fieldData } = useField(field.id);
        return (
          <HStack>
            <OptionsField
              name={field.id}
              label={field.name}
              options={fieldData?.selectFieldOptions}
              isMultiple
              isDisabled={isInstructor || field.isApproved}
            />
            <Feedback field={field} isInstructor={isInstructor} />
          </HStack>
        );
      };
      return <CheckBoxField />;
    }

    case INPUT_TYPES.Radio.toUpperCase(): {
      const RadioButtonField = () => {
        const { data: fieldData } = useField(field.id);
        return (
          <HStack>
            <OptionsField
              name={field.id}
              label={field.name}
              options={fieldData?.selectFieldOptions}
              isDisabled={isInstructor || field.isApproved}
            />
            <Feedback field={field} isInstructor={isInstructor} />
          </HStack>
        );
      };
      return <RadioButtonField />;
    }
    default:
      return null;
  }
};
