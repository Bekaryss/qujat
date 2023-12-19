import React, { useEffect } from "react";
import { Box } from "@mui/material";
import TableActs from "../../components/Table/TableActs/TableActs";
import "./Acts.css";

const Acts = () => {
  return (
    <Box sx={{ width: "100%", display: "flex", flexDirection: "column" }}>
      <h1>Нормативтiк құқықтық актiлер</h1>
      <div className="table_container">
        <TableActs />
      </div>
    </Box>
  );
};

export default Acts;
