import { TextField } from "components/forms/TextField";
import { DescriptionTextArea } from "components/forms/DescriptionTextArea";
import { FileUpload } from "components/forms/FileUpload";
import { fetchKeys } from "api/experiments";
import { DraggableList } from "components/forms/DraggableList";
import { CheckBox } from "components/forms/CheckBox";
import { INPUT_TYPES } from "constants/input-types";

export const ExperimentField = ({ field, experimentId }) => {
  const { downloadFile } = fetchKeys;
  switch (field.fieldType.toUpperCase()) {
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
        <DescriptionTextArea
          name={field.id}
          title={field.name}
          placeholder={field.name}
          isRequired={field.mandatory}
        />
      );
    case INPUT_TYPES.File.toUpperCase():
      return (
        <FileUpload
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
      return <DraggableList name={field.id} label={field.name} />;

    case INPUT_TYPES.Multiple.toUpperCase():
      return (
        <CheckBox
          name={field.id}
          label={field.name}
          options={field?.selectFieldOptions}
        />
      );
    default:
      return null;
  }
};
