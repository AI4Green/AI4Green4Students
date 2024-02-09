import { INPUT_TYPES } from "constants/input-types";

const getInitialValue = (field) => {
  const {
    File,
    Text,
    Description,
    Multiple,
    Radio,
    DraggableList,
    ChemicalDisposalTable,
    Headers,
    Content,
  } = INPUT_TYPES;

  const fieldType = field.fieldType.toUpperCase();

  switch (fieldType) {
    case File.toUpperCase(): {
      const isFilePresent = `${field.id}_isFilePresent`;
      return {
        [field.id]: field.fieldResponse,
        [isFilePresent]: !!field.fieldResponse?.fileName,
      };
    }

    case Text.toUpperCase():
    case Description.toUpperCase():
      return { [field.id]: field.fieldResponse ?? "" };

    case Multiple.toUpperCase():
    case Radio.toUpperCase():
    case DraggableList.toUpperCase():
    case ChemicalDisposalTable.toUpperCase():
      return { [field.id]: field.fieldResponse ?? [] };

    case Headers.toUpperCase():
    case Content.toUpperCase():
      return {};

    default:
      return { [field.id]: field.fieldResponse };
  }
};

export const initialValues = (sectionFields) =>
  sectionFields.reduce((acc, field) => {
    return { ...acc, ...getInitialValue(field) };
  }, {});
