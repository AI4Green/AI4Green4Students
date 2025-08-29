import { useUser } from "contexts";
import { DefaultLayout } from "layouts/default";
import { ContentPage } from "pages/content";
import { NotFound } from "pages/error";
import GreenMetrics from "pages/green-metrics";
import { Home } from "pages/home";
import ReactionPredictions from "pages/reactions-predictions";
import { Navigate, Route, Routes } from "react-router-dom";

import { Account } from "./account";
import { Admin } from "./admin";
import { Project } from "./project";

const ConditionalHome = () => {
  const { user } = useUser();
  if (user) {
    return (
      <Routes>
        <Route path="/" element={<DefaultLayout />}>
          <Route index element={<Navigate to="/projects" />} />
        </Route>
      </Routes>
    );
  }
  return <Home />;
};

export const Root = () => {
  return (
    <Routes>
      <Route index element={<ConditionalHome />} />

      <Route path="/" element={<DefaultLayout />}>
        <Route path="metrics" element={<GreenMetrics />} />
        <Route path="reaction-predictions" element={<ReactionPredictions />} />
        <Route
          path="greenchemistry"
          element={<ContentPage contentKey={"greenchemistry"} />}
        />
        <Route path="about" element={<ContentPage contentKey={"about"} />} />

        <Route
          path="documentation"
          element={<ContentPage contentKey={"documentation"} />}
        />

        <Route path="projects/*" element={<Project />} />

        <Route path="account/*" element={<Account />} />

        <Route path="admin/*" element={<Admin />} />

        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
};
