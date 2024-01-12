import { Grid, GridItem } from "@chakra-ui/react";
import { Outlet } from "react-router-dom";
import { Sidebar } from "components/sidebar/Sidebar";
import { useLocationStateToast } from "helpers/hooks/useLocationStateToast";
import { Footer } from "components/Footer";

export const DefaultLayout = ({ toastDefaults = { position: "top" } }) => {
  useLocationStateToast(toastDefaults);

  return (
    <Grid templateRows="1fr auto" height="100vh">
      <Sidebar>
        <Outlet />
      </Sidebar>
      <GridItem>
        <Footer />
      </GridItem>
    </Grid>
  );
};
