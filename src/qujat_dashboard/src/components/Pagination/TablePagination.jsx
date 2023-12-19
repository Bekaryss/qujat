import React, { useEffect } from "react";
import { Pagination, useMediaQuery, useTheme, styled } from "@mui/material";

const StyledPagination = styled(Pagination)(({ theme }) => ({
  "& .MuiPaginationItem-root": {
    color: "#000",
    "&:hover": {
      backgroundColor: "var(--main-color-300) !important",
    },
  },
  "& .Mui-selected": {
    color: "#000",
    backgroundColor: "var(--main-color-300) !important",
    "&:hover": {
      backgroundColor: "var(--main-color-300) !important", // Change to your desired active hover color
    },
  },
}));

const TablePagination = ({
  totalSize,
  pageIndex,
  pageSize,
  handleChangePage,
}) => {
  const theme = useTheme();
  const isSmallScreen = useMediaQuery(theme.breakpoints.down("sm"));

  //  проверка на null и undefined
  const count = pageSize && totalSize ? Math.ceil(totalSize / pageSize) : 0;

  useEffect(() => {
    // console.log(pageSize);
    // console.log(totalSize);
  }, [pageSize, totalSize, count]);

  return (
    <StyledPagination
      count={count}
      page={pageIndex + 1}
      onChange={(_, value) => handleChangePage(_, value)}
      size={isSmallScreen ? "small" : "medium"}
      variant="outlined"
      shape="rounded"
    />
  );
};

export default TablePagination;
