import { Heading } from "@chakra-ui/react";
import { TextField } from "components/forms/TextField";
import { TextAreaField } from "components/forms/TextAreaField";
import { FileUploadField } from "components/forms/FileUploadField";
import { fetchKeys } from "api/experiments";
import { DraggableListField } from "components/forms/DraggableListField";
import { INPUT_TYPES } from "constants/input-types";
import { OptionsField } from "components/forms/OptionsField";

export const ExperimentField = ({ field, experimentId }) => {
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
        <TextField
          name={field.id}
          label={field.name}
          isRequired={field.mandatory}
          placeholder={field.name}
        />
      );
    case INPUT_TYPES.Description.toUpperCase():
      return (
        <TextAreaField
          name={field.id}
          title={field.name}
          placeholder={field.name}
          isRequired={field.mandatory}
        />
      );
    case INPUT_TYPES.File.toUpperCase():
      return (
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
        />
      );
    case INPUT_TYPES.DraggableList.toUpperCase():
      return <DraggableListField name={field.id} label={field.name} />;

    case INPUT_TYPES.Multiple.toUpperCase():
      return (
        <OptionsField
          name={field.id}
          label={field.name}
          options={field?.selectFieldOptions}
          isMultiple
        />
      );

    case INPUT_TYPES.Radio.toUpperCase():
      return (
        <OptionsField
          name={field.id}
          label={field.name}
          options={field?.selectFieldOptions}
        />
      );
    default:
      return null;
  }
};
