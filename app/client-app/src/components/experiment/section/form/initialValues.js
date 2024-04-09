import { INPUT_TYPES } from "constants/input-types";
import { useBackendApi } from "contexts/BackendApi";

const getInitialValue = (field, recordId, sectionId) => {
  const {
    File,
    Text,
    Description,
    Multiple,
    Radio,
    DraggableList,
    ChemicalDisposalTable,
    Header,
    Content,
    ProjectGroupPlanTable,
    ProjectGroupHazardTable,
  } = INPUT_TYPES;

  const fieldType = field.fieldType.toUpperCase();

  switch (fieldType) {
    case File.toUpperCase(): {
      const { sections: action } = useBackendApi();
      return {
        [field.id]: Array.isArray(field.fieldResponse)
          ? field.fieldResponse.map((file) => ({
              ...file,
              download: async () =>
                await action.downloadSectionFile(
                  sectionId,
                  recordId,
                  file.location,
                  file.name
                ),
            }))
          : [],
      };
    }

    case Text.toUpperCase():
    case Description.toUpperCase():
      return { [field.id]: field.fieldResponse ?? "" };

    case Multiple.toUpperCase():
    case Radio.toUpperCase():
    case DraggableList.toUpperCase():
    case ChemicalDisposalTable.toUpperCase():
    case ProjectGroupPlanTable.toUpperCase():
    case ProjectGroupHazardTable.toUpperCase():
      return {
        [field.id]: !field.fieldResponse ? [] : field.fieldResponse,
      };

    case Header.toUpperCase():
    case Content.toUpperCase():
      return {};

    default:
      return { [field.id]: field.fieldResponse };
  }
};

export const initialValues = (sectionFields, recordId, sectionId) =>
  sectionFields.reduce((acc, field) => {
    return { ...acc, ...getInitialValue(field, recordId, sectionId) };
  }, {});
