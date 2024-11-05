import { Image } from "@chakra-ui/react";
import { useBackendApi } from "contexts/BackendApi";
import { useObjectUrl } from "helpers/hooks/useObjectUrl";
import { useEffect, useState } from "react";

// Component for displaying an field response image.
export const FieldImage = ({ sectionId, recordId, image }) => {
  const { location, name } = image;
  const [blob, setBlob] = useState(null);
  const objectUrl = useObjectUrl(blob);
  const { sections: action } = useBackendApi();

  useEffect(() => {
    const fetchImage = async () => {
      try {
        const response = await action.downloadSectionFile(
          sectionId,
          recordId,
          location,
          name
        );
        const blob = await response.blob();
        setBlob(blob);
      } catch (error) {
        console.error("Error fetching image.", error);
      }
    };

    fetchImage();
  }, []);

  return <Image src={objectUrl} h={32} my={4} objectFit="contain" />;
};
