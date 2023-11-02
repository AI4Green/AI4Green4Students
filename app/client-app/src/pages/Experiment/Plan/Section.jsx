// TODO: generate section fields

import { Layout } from "components/experiment/Layout";
import { useParams } from "react-router-dom";
import { Header } from "./Overview";

export const Section = () => {
  const { experimentId, sectionId } = useParams(); // TODO: use sectionId to get section fields

  return (
    <Layout>
      <Header
        header="Section"
        experimentTitle="Experiment Title"
        projectName="Project Name"
      />
    </Layout>
  );
};
