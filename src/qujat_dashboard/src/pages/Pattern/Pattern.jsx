import React from "react";
import { Box } from "@mui/material";

import TablePattern from "../../components/Table/TablePattern/TablePattern";

const Pattern = () => {
  return (
    <Box sx={{ width: "100%", display: "flex", flexDirection: "column" }}>
      <h1>Құжат үлгілері</h1>
      <div className="table_container">
        <TablePattern />
      </div>
    </Box>
  );
};

export default Pattern;
